using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private MazeGenerator _generator;
    [SerializeField] private PlayerObserver _playerObserver;
    
    private void Start()
    {
        Restart();
    }
    
    private void Restart()
    {
        var maze = _generator.Generate();
        maze.ExitTrigger.OnTriggerEnter += delegate { Restart(); };

        Vector3 spawnPosition = maze.StartCell.Position;
        _playerObserver.SpawnPlayer(spawnPosition);
    }
}
