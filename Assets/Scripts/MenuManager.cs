using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<Mode> modes;

    public void OpenMenuAtlas()
    {
        SetModeVisibility(0);
    }

    public void OpenMenuCT()
    {
        SetModeVisibility(1);
    }

    public void OpenMenuMR()
    {
        SetModeVisibility(2);
    }

    public void OpenMenuMU()
    {
        SetModeVisibility(3);
    }

    public void OpenMenuAnatomy()
    {
        SetModeVisibility(4);
    }

    private void SetModeVisibility(int index)
    {
        for (var i = 0; i < modes.Count; i++)
        {
            modes[i].menu.SetActive(i == index);
            modes[i].item.SetActive(i == index);
        }
    }
}

[Serializable]
public struct Mode
{
    public GameObject menu;
    public GameObject item;
}