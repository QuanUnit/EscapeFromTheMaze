using System.Collections.Generic;
using MazeGame.Maze.Environment;
using UnityEngine;

namespace MazeGame.Maze.Algorithms
{
    public abstract class ScoreFiller : MazeFillingAlgorithm
    {
        [SerializeField] protected GameObject _scorePointPrefab;

        protected void CreateScorePoints(List<MazeCell> branch, int startIndex, int length, Transform container)
        {
            MazeCell current = branch[startIndex];
            MazeCell next;

            for (int i = 0; i < length - 1; i++)
            {
                next = branch[startIndex + i + 1];

                Vector3 averagePosition = (next.Position + current.Position) / 2;

                MonoBehaviour storedObject = next.StoredObjects.Find(mono => mono.GetType() == typeof(ScorePoint));

                if (storedObject == null)
                {
                    GameObject scoreGO1 = Instantiate(_scorePointPrefab, averagePosition, Quaternion.identity, container);
                    GameObject scoreGO2 = Instantiate(_scorePointPrefab, next.Position, Quaternion.identity, container);

                    next.StoredObjects.Add(scoreGO2.GetComponentInChildren<ScorePoint>());
                }

                current = next;
            }
        }
    }
}