using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScoreFilingAlgorithm : MazeFillingAlgorithm
{
    [SerializeField] private ScorePoint _scorePointPrefab;

    [SerializeField] private int _maxScoreLineLength;

    public override void FillMaze(Maze maze)
    {
        foreach (var branch in maze.Branches)
        {
            int start = Random.Range(0, branch.Key.Count);
            int length = Mathf.Min(Random.Range(1, _maxScoreLineLength + 1), branch.Key.Count - start);

            CreateScorePoints(branch.Key, start, length, maze.Container);
        }
    }

    private void CreateScorePoints(List<MazeCell> branch, int startIndex, int length, Transform container)
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
