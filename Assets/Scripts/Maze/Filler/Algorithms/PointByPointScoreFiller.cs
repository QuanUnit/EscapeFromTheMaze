using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PointByPointScoreFiller : ScoreFiller
{
    public override void FillMaze(Maze maze)
    {
        foreach (var branch in maze.Branches)
        {
            if (branch.Type == BranchType.Main)
            {
                CreateScorePoints(branch.Path, 2, branch.Path.Count - 2, maze.Container);   
            }
            else
            {
                List<MazeCell> subBranchPath = new List<MazeCell>();
                for(int i = 0; i < branch.Path.Count; i++)
                {
                    MazeCell cell = branch.Path[i];
                    
                    subBranchPath.Add(cell);
                    MonoBehaviour storedObject = cell.StoredObject;
                    
                    if (storedObject == null) continue;
                    
                    if (storedObject.GetType() == typeof(MazeButton))
                    {
                        CreateScorePoints(subBranchPath, 0, subBranchPath.Count, maze.Container);
                        break;
                    }
                }
            }
        }
    }
}
