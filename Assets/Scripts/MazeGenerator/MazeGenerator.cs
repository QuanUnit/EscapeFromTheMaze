using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeHeight;

    [SerializeField] private MazeWall _wallPrefab;
    [SerializeField] private Transform _mazeContainer;

    public void Generate()
    {

    }

    private MazeCell[,] InitializeGrid()
    {
        MazeCell[,] grid = new MazeCell[_mazeHeight, _mazeWidth];

        return grid;
    }
}
