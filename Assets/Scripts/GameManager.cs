using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public UnityEvent OnGameWon;
    public UnityEvent OnGameLost;

    [SerializeField] private MazeGenerator _generator;
    [SerializeField] private PlayerObserver _playerObserver;
    
    private void Start()
    {
        RestartGame();
    }
    
    public void RestartGame()
    {
        var maze = _generator.Generate();
        maze.ExitTrigger.OnTriggerEnter += delegate { WinGame(); };

        Vector3 spawnPosition = maze.StartCell.Position;
        _playerObserver.SpawnPlayer(spawnPosition);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    private void WinGame()
    {
        _playerObserver.DeletePlayer();
        OnGameWon?.Invoke();
    }

    private void LoseGame()
    {
        _playerObserver.DeletePlayer();
        OnGameLost?.Invoke();
    }
}
