using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CtSliceManager : MonoBehaviour
{
    public string baseFolderPath = "Assets/CT_Slices/";
    public Renderer planeRenderer;
    public Transform slicePlane;
    public Transform modelTarget;
    public Slider sliceSlider;
    public Text axisText;

    private enum Axis { X, Y, Z }
    [SerializeField] private Axis currentAxis = Axis.Z;

    private readonly Dictionary<Axis, List<Texture2D>> _sliceTextures = new();
    [SerializeField] private int currentSliceIndex;
    private bool _isAnimating;
    private Vector3 _initialLocalPosition;

    private void Start()
    {
        LoadSlices(Axis.X, "X");
        LoadSlices(Axis.Y, "Y");
        LoadSlices(Axis.Z, "Z");

        _initialLocalPosition = slicePlane.localPosition;
        UpdateTexture();
        UpdateUI();
    }

    private void Update()
    {
        UpdateTexture();
        UpdatePlanePosition();
    }

    private void LoadSlices(Axis axis, string folderName)
    {
        var folderPath = Path.Combine(baseFolderPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        var files = Directory.GetFiles(folderPath, "image_*.png");

        System.Array.Sort(files, (a, b) =>
        {
            var numA = int.Parse(Path.GetFileNameWithoutExtension(a).Split('_')[1]);
            var numB = int.Parse(Path.GetFileNameWithoutExtension(b).Split('_')[1]);
            return numA.CompareTo(numB);
        });

        var textures = new List<Texture2D>();
        foreach (var file in files)
        {
            var tex = LoadTexture(file);
            if (tex != null) textures.Add(tex);
        }

        _sliceTextures[axis] = textures;

        if (axis == currentAxis && sliceSlider != null)
        {
            sliceSlider.maxValue = textures.Count - 1;
            sliceSlider.value = 0;
        }
    }

    private Texture2D LoadTexture(string filePath)
    {
        var fileData = File.ReadAllBytes(filePath);
        var tex = new Texture2D(2, 2);
        if (tex.LoadImage(fileData))
            return tex;
        return null;
    }

    public void ChangeSlice(float index)
    {
        var newIndex = Mathf.Clamp((int)index, 0, _sliceTextures[currentAxis].Count - 1);
        if (newIndex != currentSliceIndex)
        {
            currentSliceIndex = newIndex;
            StartCoroutine(FadeTexture(_sliceTextures[currentAxis][currentSliceIndex]));
            UpdatePlanePosition();
        }
    }

    private IEnumerator FadeTexture(Texture2D newTexture)
    {
        if (_isAnimating) yield break;
        _isAnimating = true;

        var mat = planeRenderer.material;
        var duration = 0.3f;
        var elapsedTime = 0f;
        var color = mat.color;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(1, 0, elapsedTime / duration);
            mat.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mat.mainTexture = newTexture;

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(0, 1, elapsedTime / duration);
            mat.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _isAnimating = false;
    }

    private void UpdatePlanePosition()
    {
        var step = 0.002f;

        var offset = Vector3.zero;
        switch (currentAxis)
        {
            case Axis.X:
                slicePlane.rotation = Quaternion.Euler(0, 0, 90f);
                break;
            case Axis.Y:
                slicePlane.rotation = Quaternion.identity;
                break;
            case Axis.Z:
                slicePlane.rotation = Quaternion.Euler(90f, 0, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (currentAxis == Axis.X) offset = new Vector3(step * currentSliceIndex, 0, 0);
        else if (currentAxis == Axis.Y) offset = new Vector3(0, step * currentSliceIndex, 0);
        else if (currentAxis == Axis.Z) offset = new Vector3(0, 0, step * currentSliceIndex);

        slicePlane.localPosition = _initialLocalPosition + offset;
    }

    public void SetAxis(string axis)
    {
        if (axis == "X") currentAxis = Axis.X;
        else if (axis == "Y") currentAxis = Axis.Y;
        else if (axis == "Z") currentAxis = Axis.Z;

        currentSliceIndex = 0;
        sliceSlider.maxValue = _sliceTextures[currentAxis].Count - 1;
        sliceSlider.value = 0;
        UpdateTexture();
        UpdateUI();
        UpdatePlanePosition();
    }

    private void UpdateTexture()
    {
        if (_sliceTextures.ContainsKey(currentAxis) && _sliceTextures[currentAxis].Count > 0)
        {
            planeRenderer.material.mainTexture = _sliceTextures[currentAxis][currentSliceIndex];
        }
    }

    private void UpdateUI()
    {
        if (axisText != null)
            axisText.text = "���: " + currentAxis.ToString();
    }
}
