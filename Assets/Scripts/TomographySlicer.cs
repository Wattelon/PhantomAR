using System;
using System.Collections.Generic;
using UnityEngine;

public class TomographySlicer : MonoBehaviour
{
    [SerializeField] private TomographySO tomographyData;
    [SerializeField] private Tomography currentTomography;
    [SerializeField] private Axis currentAxis;
    [SerializeField][Range(0, 299)] private int currentIndex;
    [SerializeField] private Vector3 dimensions;
    [SerializeField] private List<Transform> pivotPoints;
    
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        UpdateSlice();
    }

    private void UpdateSlice()
    {
        _meshRenderer.material.mainTexture = tomographyData.Slices[currentTomography][currentAxis][currentIndex];
        
        transform.SetParent(pivotPoints[(int)currentAxis], false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        var step = dimensions[(int)currentAxis] / 300;
        transform.localPosition = Vector3.forward * (currentIndex * step);
    }
}

public enum Tomography
{
    Computer,
    MagneticResonance
}

public enum Axis
{
    X,
    Y,
    Z
}