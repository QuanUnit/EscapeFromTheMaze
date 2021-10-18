using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Test : MazeFillingAlgorithm
{
    [SerializeField] private GameObject test;
    public override void FillMaze(Maze maze)
    {
        foreach (var item in maze.MazeGrid)
        {
            if (item.ContactWalls.Count >= 4)
            {
                Debug.Log($"Walls count = {item.ContactWalls.Count} | {item.Position} | {item.DistanceToStart}");
            }
        }

        //foreach (var item in maze.Branches)
        //{
        //    if (item.Value == Maze.BranchType.Main) continue;

        //    foreach (var cell in item.Key)
        //    {
        //        GameObject.Instantiate(test, cell.Position, Quaternion.identity);
        //    }
        //}
    }
}
