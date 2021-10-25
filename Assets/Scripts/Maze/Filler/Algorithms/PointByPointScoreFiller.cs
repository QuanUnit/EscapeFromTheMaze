using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PointByPointScoreFiller : ScoreFiller
{
    public override void FillMaze(Maze maze)
    {
        CreateScorePoints(maze.MainBranch.Path, 1, maze.MainBranch.Path.Count - 1, maze.Container);

        foreach (var branch in maze.SecondaryBranches)
        {
            foreach (var cell in branch.Path)
            {
                if (cell.StoredObjects.Count == 0) continue;
                
                MonoBehaviour storedObject = cell.StoredObjects.Find(mono => mono != null &&
                                                                             mono.GetType() == typeof(MazeButton));
                
                if (storedObject != null)
                {
                    List<MazeCell> scorePath = GetPathFromButtonToBranch(maze.MainBranch, cell);
                    CreateScorePoints(scorePath, 0, scorePath.Count, maze.Container);
                    break;
                }
            }
        }
    }

    private List<MazeCell> GetPathFromButtonToBranch(Branch<MazeCell> targetBranch, MazeCell buttonCell)
    {
        Stack<MazeCell> path = new Stack<MazeCell>();

        List<MazeCell> visited = new List<MazeCell>();
        
        MazeCell current = buttonCell;
        
        while (true)
        {
            if (TryMoveNext(current, out var next))
            {
                path.Push(current);

                if (targetBranch.Path.Contains(current))
                {
                    List<MazeCell> output = new List<MazeCell>(path);
                    return output;
                }
                current = next;
            }
            else
            {
                current = path.Pop();
            }
        }

        bool TryMoveNext(MazeCell cur, out MazeCell next)
        {
            next = default;
            
            foreach (var subject in cur.ConnectedSubjects)
            {
                if (visited.Contains(subject) == false)
                {
                    next = (MazeCell)subject;
                    visited.Add(next);
                    return true;
                }
            }

            return false;
        }
    }
}
