using MazeGame.Maze;
using MazeGame.Player;
using MazeGame.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MazeGame
{
    public class GameManager : Singleton<GameManager>
    {
        public UnityEvent OnGameWon;
        public UnityEvent OnGameLost;

        [SerializeField] private MazeGenerator _generator;
        [SerializeField] private PlayerObserver _playerObserver;
        [SerializeField] private Timer _timer;

        private void Start()
        {
            RestartGame();
        }

        public void GeneralRestart() => SceneManager.LoadScene(gameObject.scene.buildIndex);

        public void RestartGame()
        {
            var maze = _generator.Generate();
            maze.ExitTrigger.OnTriggerEnter.AddListener(delegate { WinGame(); });

            Vector3 spawnPosition = maze.StartCell.Position;
            _playerObserver.SpawnPlayer(spawnPosition);

            _timer.Launch();
        }

        public void ExitApplication()
        {
            Application.Quit();
        }

        public void WinGame()
        {
            _playerObserver.DeletePlayer();
            _timer.Stop();
            OnGameWon?.Invoke();
        }

        public void LoseGame()
        {
            _playerObserver.DeletePlayer();
            _timer.Stop();
            OnGameLost?.Invoke();
        }

        public void BackToMenu() => SceneManager.LoadScene(0);

        public void ApplyMoney(int value)
        {
            GlobalWallet.Value += value;
            Debug.Log(value);
        }
    }
}