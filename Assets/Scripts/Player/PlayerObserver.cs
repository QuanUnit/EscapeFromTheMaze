using System.Collections.Generic;
using MazeGame.UI;
using UnityEngine;

namespace MazeGame.Player
{
    public class PlayerObserver : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerPrefab;
        [SerializeField] private List<PropertyViewer> _walletViewers;

        private Wallet _wallet;
        private PlayerController _activePlayer;

        public PlayerController SpawnPlayer(Vector3 position)
        {
            if (_activePlayer != null)
                DeletePlayer();

            GameObject playerGO = Instantiate(_playerPrefab.gameObject, position, Quaternion.identity, transform);

            _activePlayer = playerGO.GetComponent<PlayerController>();
            _wallet = playerGO.GetComponent<Wallet>();

            ComponentInitialize();
            _wallet.Reset();

            return _activePlayer;
        }

        public void DeletePlayer()
        {
            Destroy(_activePlayer.gameObject);
        }
        private void ComponentInitialize()
        {
            foreach (var viewer in _walletViewers)
            {
                viewer.Initialize(_wallet);
            }
        }
    }
}