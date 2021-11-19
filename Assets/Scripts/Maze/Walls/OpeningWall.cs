using System;
using UnityEngine;

namespace MazeGame.Maze.Environment
{
    [RequireComponent(typeof(Animator))]
    public class OpeningWall : SimpleWall
    {
        public event Action OnOpened;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Open()
        {
            _animator.SetTrigger("Open");
            OnOpened?.Invoke();
        }
    }
}