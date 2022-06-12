using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuRoot : MonoBehaviour
{
    [SerializeField] private ShopPanel _shopPanel;

    public void ShowShopPanel()
    {
        _shopPanel.gameObject.SetActive(true);
    }

    public void HideShopPanel()
    {
        _shopPanel.gameObject.SetActive(false);
    }

    public void PlayClick()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitApplication()
    {
        Debug.Log("Application ends self execution");
        Application.Quit();
    }
}
