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
            if (branch.Type == BranchType.Main) continue;

            int start = Random.Range(0, branch.Path.Count);
            int length = Mathf.Min(Random.Range(1, _maxScoreLineLength + 1), branch.Path.Count - start);

            CreateScorePoints(branch.Path, start, length, maze.Container);
        }
    }
}
