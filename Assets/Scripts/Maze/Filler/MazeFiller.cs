using System.Collections.Generic;
using UnityEngine;

namespace MazeGame.Maze.Algorithms
{
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
}