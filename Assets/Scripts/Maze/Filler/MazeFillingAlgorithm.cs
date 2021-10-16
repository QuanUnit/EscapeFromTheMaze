using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MazeFillingAlgorithm : ScriptableObject
{
    public abstract void FillMaze(Maze maze);
}
