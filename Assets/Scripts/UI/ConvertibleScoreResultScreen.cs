using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BestOfTheNest.UI
{
    public class ConvertibleScoreResultScreen : MonoBehaviour
    {
        public UnityEvent<int> ConvertFinished;

        public float MultiplyValue = 100;

        [SerializeField] private TMP_Text _scoreSource;
        [SerializeField] private TMP_Text _scoreTargetField;
        [SerializeField] private Button[] _lockableButtons;
        /// <summary>
        /// Transited scores in 1 second
        /// </summary>
        [SerializeField] [Min(0)] private float _transitionSpeed = 8f;
        [SerializeField] [Min(0)] private float _acceleration = 0.02f;
        [SerializeField] [Min(0)] private float _transitionStartDelay = 1f;

        private Coroutine _transitionProcess;
        private float _startSpeed;

        private void Awake()
        {
            _startSpeed = _transitionSpeed;
        }

        private void OnEnable()
        {
            foreach (var btn in _lockableButtons) btn.interactable = false;

            Action<int> transitionFinish = value =>
            {
                foreach (var btn in _lockableButtons) btn.interactable = true;
                ConvertFinished?.Invoke(value);
            };

            _transitionProcess = StartCoroutine(TransitScores(transitionFinish));
        }

        public void OnDisable()
        {
            ClearFields();
        }

        private IEnumerator TransitScores(Action<int> callback)
        {
            yield return new WaitForSeconds(_transitionStartDelay);

            int sourceScores = int.Parse(_scoreSource.text);
            int startSourceScores = sourceScores;
            float targetScores = 0;
            int tickCount = 1; while (sourceScores > 0)
            {
                float deltaTime = 1f / _transitionSpeed;
                float sqrtAcc = (_acceleration * _acceleration);
                tickCount += (int)sqrtAcc;

                _transitionSpeed += sqrtAcc;
                sourceScores -= tickCount;

                targetScores += (tickCount * MultiplyValue);

                if (sourceScores < 0) sourceScores = 0;
                if (targetScores > startSourceScores * MultiplyValue) targetScores = (int)(startSourceScores * MultiplyValue);

                ApplyScoresValues(sourceScores, (int)targetScores);
                yield return new WaitForSeconds(deltaTime);
            }

            _transitionSpeed = _startSpeed;
            callback?.Invoke((int)targetScores);
        }

        private void ApplyScoresValues(int source, int target)
        {
            _scoreSource.text = source.ToString();
            _scoreTargetField.text = target.ToString();
        }

        private void ClearFields()
        {
            _scoreTargetField.text = "0";
        }
    }
}