using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PointByPointScoreFiller : ScoreFiller
{
    public override void FillMaze(Maze maze)
    {
        foreach (var branch in maze.Branches)
        {
            if (branch.Value == Maze.BranchType.Main)
            {
                CreateScorePoints(branch.Key, 2, branch.Key.Count - 2, maze.Container);   
                return;
            }
        }
    }
}
