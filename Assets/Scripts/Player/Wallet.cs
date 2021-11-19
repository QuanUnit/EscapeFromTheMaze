using System;
using MazeGame.Abstract;
using UnityEngine;

namespace MazeGame.Player
{
    public class Wallet : MonoBehaviour, IPropertyChangeNotifier
    {
        public event Action<object> PropertyOnChanged;

        private int _amount;

        public void Add(int value)
        {
            _amount += value;
            PropertyOnChanged?.Invoke(_amount);
        }

        public void Reset()
        {
            _amount = 0;
            PropertyOnChanged?.Invoke(_amount);
        }
    }
}