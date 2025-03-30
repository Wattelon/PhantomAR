using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using UnityEditor;

public class TomographySO : ScriptableObject
{
    private protected Tomography tomography;
    
    private static readonly int TomographyVolume = Shader.PropertyToID("_TomographyVolume");
    
    private Dictionary<Axis, List<Texture2D>> _slices;
    private List<DicomFile> _dicomFiles;
    private ushort[,,] _volumeData;
    private Color[] _voxels;
    private int _width;
    private int _height;
    private int _depth;
    private float _rescaleSlope;
    private float _rescaleIntercept;

    public Dictionary<Axis, List<Texture2D>> Slices => _slices;

    private void OnValidate()
    {
        LoadTextures();
        //SetGlobalTexture3D();
    }

    private protected virtual void Reset()
    {
        ClearTextures();
        SetupTomography();
    }

    private void SetupTomography()
    {
        LoadDicomFiles();
        SetDimensions();
        SetVolumeData();
        GenerateSlices();
    }

    private void LoadDicomFiles()
    {
        _dicomFiles = new List<DicomFile>();
        var fullPath = Path.Combine(Application.streamingAssetsPath, $"Tomography/{tomography.ToString()}");
        var files = Directory.GetFiles(fullPath, "*.dcm").ToList();
        files.Sort();

        foreach (var file in files)
        {
            _dicomFiles.Add(DicomFile.Open(file));
        }
    }

    private void SetDimensions()
    {
        var pixelData = DicomPixelData.Create(_dicomFiles[0].Dataset);
        _width = pixelData.Width;
        _height = pixelData.Height;
        _depth = _dicomFiles.Count;
        _rescaleSlope = _dicomFiles[0].Dataset.GetSingleValueOrDefault(DicomTag.RescaleSlope, 1);
        _rescaleIntercept = _dicomFiles[0].Dataset.GetSingleValueOrDefault(DicomTag.RescaleIntercept, -1024);
    }

    private void SetVolumeData()
    {
        _volumeData = new ushort[_width, _height, _depth];
        _voxels = new Color[_width * _height * _depth];
        
        for (var z = 0; z < _depth; z++)
        {
            var file = _dicomFiles[z];
            var image = new DicomImage(file.Dataset).RenderImage();

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    var pixelValue = (ushort)image.GetPixel(x, y).Value;
                    _volumeData[x, y, z] = pixelValue;
                    var index = (z * _height + y) * _width + x;
                    var intensity = pixelValue / (float)ushort.MaxValue;
                    _voxels[index] = new Color(intensity, intensity, intensity, 1);
                }
            }
        }
    }

    private void GenerateSlices()
    {
        foreach (var axis in Enum.GetValues(typeof(Axis)))
        {
            var dimension = 2 - (int)axis;
            for (var i = 0; i < _volumeData.GetLength(dimension); i++)
            {
                var texture = CreateTexture((Axis)axis, i);
                SaveTexture(texture, (Axis)axis, i);
            }
        }
    }
    
    private Texture2D CreateTexture(Axis axis, int index)
    {
        int textureWidth, textureHeight;
        Func<int, int, ushort> getValue;

        switch (axis)
        {
            case Axis.Axial:
                textureWidth = _width;
                textureHeight = _height;
                getValue = (x, y) => _volumeData[x, y, index];
                break;

            case Axis.Sagittal:
                textureWidth = _depth;
                textureHeight = _height;
                getValue = (x, y) => _volumeData[index, y, x];
                break;

            case Axis.Coronal:
                textureWidth = _width;
                textureHeight = _depth;
                getValue = (x, y) => _volumeData[x, index, y];
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }
        
        var colors = new Color[textureWidth * textureHeight];
        for (var y = 0; y < textureHeight; y++)
        {
            for (var x = 0; x < textureWidth; x++)
            {
                var normalizedValue = getValue(x, y) / (float)ushort.MaxValue;
                colors[y * textureWidth + x] = new Color(normalizedValue, normalizedValue, normalizedValue, 1);
            }
        }
        var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA64, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixels(colors);
        texture.Apply();
        
        return texture;
    }

    private void SaveTexture(Texture2D texture, Axis axis, int index)
    {
        AssetDatabase.CreateAsset(texture, $"Assets/Resources/Tomography/{tomography.ToString()}/{axis.ToString()}/{index}.asset");
    }

    private void LoadTextures()
    {
        if (_slices != null) return;
        _slices = new Dictionary<Axis, List<Texture2D>>();
        
        foreach (var axis in Enum.GetValues(typeof(Axis)))
        {
            _slices[(Axis)axis] = new List<Texture2D>();
            var guids = AssetDatabase.FindAssets("t:Texture2D", new []{$"Assets/Resources/Tomography/{tomography.ToString()}/{axis}"});
            foreach (var guid in guids)
            {
                _slices[(Axis)axis].Add(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid)));
            }
        }
    }

    private void ClearTextures()
    {
        AssetDatabase.DeleteAssets(new[] {$"Assets/Resources/Tomography/{tomography.ToString()}/"}, new List<string>());
        AssetDatabase.CreateFolder("Assets/Resources/Tomography", tomography.ToString());
        foreach (var axis in Enum.GetValues(typeof(Axis)))
        {
            AssetDatabase.CreateFolder($"Assets/Resources/Tomography/{tomography.ToString()}", axis.ToString());
        }
    }

    private void SetGlobalTexture3D()
    {
        var texture = new Texture3D(_width, _height, _depth, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        
        texture.SetPixels(_voxels);
        texture.Apply();
        
        Shader.SetGlobalTexture(TomographyVolume, texture);
        Debug.Log(_voxels[_voxels.Length / 2]);
    }
}
