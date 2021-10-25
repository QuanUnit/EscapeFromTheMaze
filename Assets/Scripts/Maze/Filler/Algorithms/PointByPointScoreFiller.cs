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
            }
            else
            {
                List<MazeCell> subBranch = new List<MazeCell>();
                for(int i = branch.Key.Count - 1; i >= 0; i--)
                {
                    MazeCell cell = branch.Key[i];
                    
                    subBranch.Add(cell);
                    MonoBehaviour storedObject = cell.StoredObject;
                    
                    if (storedObject == null) continue;
                    
                    if (storedObject.GetType() == typeof(MazeButton))
                    {
                        CreateScorePoints(subBranch, 0, subBranch.Count, maze.Container);
                        break;
                    }
                }
            }
        }
    }
}
