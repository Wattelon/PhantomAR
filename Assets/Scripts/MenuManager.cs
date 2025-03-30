using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuAtlas;
    [SerializeField] private GameObject menuCT;
    [SerializeField] private GameObject menuMR;
    [SerializeField] private GameObject menuUS;
    [SerializeField] private GameObject menuAnatomy;

    public void OpenMenuAtlas()
    {
        menuAtlas.SetActive(true);
        menuCT.SetActive(false);
        menuMR.SetActive(false);
        menuUS.SetActive(false);
        menuAnatomy.SetActive(false);
    }

    public void OpenMenuCT()
    {
        menuCT.SetActive(true);
        menuAtlas.SetActive(false);
        menuMR.SetActive(false);
        menuUS.SetActive(false);
        menuAnatomy.SetActive(false);
    }

    public void OpenMenuMR()
    {
        menuMR.SetActive(true);
        menuAtlas.SetActive(false);
        menuCT.SetActive(false);
        menuUS.SetActive(false);
        menuAnatomy.SetActive(false);
    }

    public void OpenMenuUS()
    {
        menuUS.SetActive(true);
        menuAtlas.SetActive(false);
        menuCT.SetActive(false);
        menuMR.SetActive(false);
        menuAnatomy.SetActive(false);
    }

    public void OpenMenuAnatomy()
    {
        menuAnatomy.SetActive(true);
        menuAtlas.SetActive(false);
        menuCT.SetActive(false);
        menuMR.SetActive(false);
        menuUS.SetActive(false);
    }
}
