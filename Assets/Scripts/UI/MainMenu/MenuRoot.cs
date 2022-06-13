using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuRoot : MonoBehaviour
{
    [SerializeField] private ShopPanel _shopPanel;
    [SerializeField] private ModesPanel _modesPanel;

    public void ShowShopPanel() => _shopPanel.gameObject.SetActive(true);

    public void HideShopPanel() => _shopPanel.gameObject.SetActive(false);

    public void ShowModesPanel() => _modesPanel.gameObject.SetActive(true);

    public void HideModesPanel() => _modesPanel.gameObject.SetActive(false);

    public void ExitApplication() => Application.Quit();

    public void StartSimpleMode() => SceneManager.LoadScene(1);

    public void StartPointByPointMode() => SceneManager.LoadScene(2);

    public void StartGuid() => SceneManager.LoadScene(3);
}