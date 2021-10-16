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
    [SerializeField] private OpeningWall _openingWallPrefab;
    [SerializeField] private MazeButton _buttonPrefab;

    public void FillMaze(Maze maze)
    {
        List<MazeGenerator.MazeCell> mainBranch = new List<MazeGenerator.MazeCell>();
        List<List<MazeGenerator.MazeCell>> secondaryBranches = new List<List<MazeGenerator.MazeCell>>();

        foreach (var branch in maze.Branches)
        {
            if (branch.Value == Maze.BranchType.Main) mainBranch = branch.Key;
            else secondaryBranches.Add(branch.Key);
        }

        for(int i = 0; i < secondaryBranches.Count; i++)
        {
            List<MazeGenerator.MazeCell> branch = secondaryBranches[Random.Range(0, secondaryBranches.Count)];

            if (branch.Count == 0)
            {
                Debug.Log("Fuckt you");
                secondaryBranches.Remove(branch);
                i--;
                continue;
            }
            MazeGenerator.MazeCell cellForButton = branch[0];

            Debug.Log("=======================");
            Debug.Log(mainBranch.Count);
            
            Debug.Log("first second branch cell = " + branch[branch.Count - 1].DistanceToStart);
            Debug.Log("button cell = " + cellForButton.DistanceToStart);
            
            Debug.Log("cell1 = " + (int) (branch[branch.Count - 1].DistanceToStart - 1));
            Debug.Log("cell2 = " + (int) branch[branch.Count - 1].DistanceToStart);
            
            MazeGenerator.MazeCell cellForWall1 = mainBranch[(int) branch[branch.Count - 1].DistanceToStart - 1];
            MazeGenerator.MazeCell cellForWall2 = mainBranch[(int) branch[branch.Count - 1].DistanceToStart];
            
            

            Debug.Log("=======================");

            OpeningWall wall = SpawnOpeningWall(cellForWall1, cellForWall2, maze.Container);
            MazeButton button = SpawnMazeButton(cellForButton, maze.Container);

            secondaryBranches.Remove(branch);
            
            button.Initialize(wall);
        }
    }

    private OpeningWall SpawnOpeningWall(MazeGenerator.MazeCell cell1, MazeGenerator.MazeCell cell2, Transform container)
    {
        Vector3 position = (cell1.Position + cell2.Position) / 2;
        float angle = Vector3.Angle(Vector3.up, cell2.Position - cell1.Position);
        GameObject wallGO = GameObject.Instantiate(_openingWallPrefab.gameObject, position,
            Quaternion.Euler(0, 0, angle), container);

        return wallGO.GetComponent<OpeningWall>();
    }

    private MazeButton SpawnMazeButton(MazeGenerator.MazeCell cell, Transform container)
    {
        MazeGenerator.WallInfo connectedWall = cell.ContactWalls[Random.Range(0, cell.ContactWalls.Count)];
        float angle = Vector3.Angle(Vector3.up, connectedWall.Position - cell.Position);
        
        GameObject buttonGO = GameObject.Instantiate(_buttonPrefab.gameObject, cell.Position, 
            Quaternion.Euler(0, 0, connectedWall.Position.x > cell.Position.x ? -angle : angle), container);
        
        buttonGO.transform.LookAt(cell.Position);

        MazeButton button = buttonGO.GetComponent<MazeButton>();

        return button;
    }
}
