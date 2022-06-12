using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsStorage : MonoBehaviour
{
    public static SkinsStorage Instance { get; private set; }

    public int CurrentSkinIndex
    {
        get => PlayerPrefs.GetInt(CurSkinKey, 0);
        set => PlayerPrefs.SetInt(CurSkinKey, value);
    }

    public Sprite CurrentSkin
    {
        get
        {
            int index = PlayerPrefs.GetInt(CurSkinKey, 0);
            return _skins[index];
        }
        set
        {
            int index = _skins.IndexOf(value);
            PlayerPrefs.SetInt(CurSkinKey, index);
        }
    }

    [SerializeField] private List<Sprite> _skins;

    private string CurSkinKey = nameof(CurSkinKey);
    private string Skin = nameof(Skin);

    private void Awake()
    {
        Instance = this;
    }

    public Sprite GetByIndex(int value)
    {
        try
        {
            return _skins[value];
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public bool GetBuyingState(int index)
    {
        int value = PlayerPrefs.GetInt($"Skin_{index}", 0);
        return value == 1 ? true : false;
    }

    public void SetBuyingState(int index, bool value)
    {
        int val = value ? 1 : 0;
        PlayerPrefs.SetInt($"Skin_{index}", val);
    }
}
