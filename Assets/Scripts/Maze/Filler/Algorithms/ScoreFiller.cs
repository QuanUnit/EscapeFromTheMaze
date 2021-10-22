using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScoreFiller: MazeFillingAlgorithm
{
    [SerializeField] protected GameObject _scorePointPrefab;
    
    protected void CreateScorePoints(List<MazeCell> branch, int startIndex, int length, Transform container)
    {
        MazeCell current = branch[startIndex];
        MazeCell next;

        Instantiate(_scorePointPrefab, branch[startIndex].Position, Quaternion.identity, container);

        for(int i = 0; i < length - 1; i++)
        {
            next = branch[startIndex + i + 1];

            Vector3 averagePosition = (next.Position + current.Position) / 2;

            Instantiate(_scorePointPrefab, averagePosition, Quaternion.identity, container);
            Instantiate(_scorePointPrefab, next.Position, Quaternion.identity, container);

            current = next;
        }
    }
}
