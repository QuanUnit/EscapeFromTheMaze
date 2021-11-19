using UnityEngine;

namespace MazeGame.Maze.Algorithms
{
    public abstract class MazeFillingAlgorithm : ScriptableObject
    {
        public abstract void FillMaze(Maze maze);
    }
}