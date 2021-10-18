using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObserver : MonoBehaviour
{
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private WalletViewer _walletViewer;

    private Wallet _wallet;
    private PlayerController _activePlayer;

    public PlayerController SpawnPlayer(Vector3 position)
    {
        if(_activePlayer != null)
            DeletePlayer();
        
        GameObject playerGO = Instantiate(_playerPrefab.gameObject, position, Quaternion.identity, transform);
        
        _activePlayer = playerGO.GetComponent<PlayerController>();
        _wallet = playerGO.GetComponent<Wallet>();
        
        ComponentInitialize();
        
        return _activePlayer;
    }

    public void DeletePlayer()
    {
        Destroy(_activePlayer.gameObject);
    }
    private void ComponentInitialize()
    {
        _walletViewer.Initialize(_wallet);
    }
}
