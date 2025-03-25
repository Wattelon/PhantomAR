using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TomographySO", menuName = "Scriptable Objects/Tomography")]
public class TomographySO : ScriptableObject
{
    private Dictionary<Tomography, Dictionary<Axis, List<Texture2D>>> _slices;
    
    public Dictionary<Tomography, Dictionary<Axis, List<Texture2D>>> Slices => _slices;

    private void OnValidate()
    {
        PopulateDictionary();
    }

    public void PopulateDictionary()
    {
        _slices = new Dictionary<Tomography, Dictionary<Axis, List<Texture2D>>>();
        foreach (Tomography tomography in Enum.GetValues(typeof(Tomography)))
        {
            _slices[tomography] = new Dictionary<Axis, List<Texture2D>>();
            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                _slices[tomography][axis] = Resources.LoadAll<Texture2D>($"Tomography/{tomography.ToString()}/{axis.ToString()}").ToList();
                _slices[tomography][axis].Sort((x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal));
                Debug.Log(_slices[tomography][axis].Count);
            }
        }
    }
}
