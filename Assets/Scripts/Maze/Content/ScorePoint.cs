using MazeGame.Player;
using MazeGame.Tools;
using UnityEngine;

namespace MazeGame.Maze.Environment
{
    [RequireComponent(typeof(Trigger))]
    public class ScorePoint : MonoBehaviour
    {
        [SerializeField] int _value;

        private Trigger _trigger;

        private void Awake()
        {
            _trigger = GetComponent<Trigger>();
        }

        private void OnEnable() => _trigger.OnTriggerEnter.AddListener(OnEnterHandler);
        private void OnDisable() => _trigger.OnTriggerEnter.RemoveListener(OnEnterHandler);

        private void OnEnterHandler(Collider2D other, Trigger sender)
        {
            if (other.TryGetComponent<Wallet>(out var wallet) == false)
                return;

            wallet.Add(_value);
            Destroy(gameObject);
        }
    }
}

