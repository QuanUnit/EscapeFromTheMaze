using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private MazeGenerator _generator;
    [SerializeField] private PlayerController _playerPrefab;

    private PlayerController _player;

    private void Start()
    {
        Restart();
    }

    private void Restart()
    {
        if (_player != null)
            Destroy(_player.gameObject);

        var maze = _generator.Generate();
        maze.ExitTrigger.OnTriggerEnter += delegate { Restart(); };

        Vector3 spawnPosition = maze.StartCell.Position;

        _player = SpawnPlayer(spawnPosition);
    }

    private PlayerController SpawnPlayer(Vector3 position)
    {
        GameObject go = Instantiate(_playerPrefab.gameObject, position, Quaternion.identity, transform);
        return go.GetComponent<PlayerController>();
    }
}
