using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Reflection;
using Unity.CodeEditor;
using UnityEngine;

[System.Serializable]
public class MazeFiller
{
    [SerializeField] List<MazeFillingAlgorithm> _mazeFillingAlgorithms;

    public void FillMaze(Maze maze)
    {
        foreach (var algorithm in _mazeFillingAlgorithms)
        {
            algorithm.FillMaze(maze);
        }
    }
}
