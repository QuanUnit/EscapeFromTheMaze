using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MazeFiller
{
    [SerializeField] private GameObject _openingWallPrefab;
    [SerializeField] private GameObject _buttonPrefab;

    [SerializeField] private int _buttonsCount;

    public void FillMaze(Maze maze)
    {
        var randomCell1 = maze.MazeGrid[Random.Range(0, maze.Height), Random.Range(0, maze.Width)];
        var randomCell2 = (MazeGenerator.MazeCell)randomCell1.ConnectedSubjects[Random.Range(0, randomCell1.ConnectedSubjects.Count)];

        SpawnOpeningWall(randomCell1, randomCell2, maze.Container);
    }

    private void SpawnOpeningWall(MazeGenerator.MazeCell cell1, MazeGenerator.MazeCell cell2, Transform container)
    {
        Vector3 position = (cell1.Position + cell2.Position) / 2;
        float angle = Vector3.Angle(Vector3.up, cell2.Position - cell1.Position);
        GameObject.Instantiate(_openingWallPrefab, position, Quaternion.Euler(0, 0, angle), container);
    }
}
