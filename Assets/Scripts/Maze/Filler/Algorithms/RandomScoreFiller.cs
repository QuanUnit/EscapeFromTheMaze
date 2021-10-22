using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RandomScoreFiller : ScoreFiller
{
    [SerializeField] private int _maxScoreLineLength;

    public override void FillMaze(Maze maze)
    {
        foreach (var branch in maze.Branches)
        {
            if (branch.Value == Maze.BranchType.Main) continue;

            int start = Random.Range(0, branch.Key.Count);
            int length = Mathf.Min(Random.Range(1, _maxScoreLineLength + 1), branch.Key.Count - start);

            CreateScorePoints(branch.Key, start, length, maze.Container);
        }
    }
}
