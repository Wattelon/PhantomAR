using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Atlas : MonoBehaviour
{
    [SerializeField] private List<AtlasPair> atlasPairs;

    public void SetSkinVisibility(bool visible)
    {
        SetAtlasVisibility(0);
    }
    
    public void SetBonesVisibility(bool visible)
    {
        SetAtlasVisibility(1);
    }
    
    public void SetArteriesVisibility(bool visible)
    {
        SetAtlasVisibility(2);
    }
    
    public void SetVeinsVisibility(bool visible)
    {
        SetAtlasVisibility(3);
    }
    
    public void SetTracheaVisibility(bool visible)
    {
        SetAtlasVisibility(4);
    }
    
    public void SetLymphNodesVisibility(bool visible)
    {
        SetAtlasVisibility(5);
    }
    
    public void SetThyroidGlandVisibility(bool visible)
    {
        SetAtlasVisibility(6);
    }

    private void SetAtlasVisibility(int index)
    {
        atlasPairs[index].meshRenderer.enabled = atlasPairs[index].toggle.isOn;
    }
}

[Serializable]
internal struct AtlasPair
{
    public Toggle toggle;
    public MeshRenderer meshRenderer;
}