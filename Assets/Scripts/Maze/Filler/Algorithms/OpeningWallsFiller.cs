using System.Collections;
using System.Collections.Generic;
using UnityEditor.Macros;
using UnityEngine;

[CreateAssetMenu]
public class OpeningWallsFiller : MazeFillingAlgorithm
{
    [SerializeField] private OpeningWall _openingWallPrefab;
    [SerializeField] private MazeButton _buttonPrefab;

    [SerializeField][Range(0, 1)] private float _wallsCountProportion;

    public override void FillMaze(Maze maze)
    {
        Branch<MazeCell> mainBranch = default;
        List<Branch<MazeCell>> secondaryBranches = new List<Branch<MazeCell>>();
        
        List<Vector3> occupiedWallsPositions = new List<Vector3>();

        foreach (var branch in maze.Branches)
        {
            if (branch.Type == BranchType.Main) mainBranch = branch;
            else secondaryBranches.Add(branch);
        }

        for (int i = 0; i < secondaryBranches.Count * _wallsCountProportion; i++)
        {
            Branch<MazeCell> branch = secondaryBranches[Random.Range(0, secondaryBranches.Count)];
            MazeCell cellForButton = branch.Path[branch.Path.Count - 1];

            int cell1Index = (int)branch.Path[0].DistanceToStart;
            int cell2Index = cell1Index + 1;

            if(cell2Index >= mainBranch.Path.Count)
            {
                secondaryBranches.Remove(branch);
                i--;
                continue;
            }    

            MazeCell cellForWall1 = mainBranch.Path[cell1Index];
            MazeCell cellForWall2 = mainBranch.Path[cell2Index];

            if(occupiedWallsPositions.Contains((cellForWall1.Position + cellForWall2.Position) / 2))
            {
                secondaryBranches.Remove(branch);
                i--;
                continue;
            }

            OpeningWall wall = SpawnOpeningWall(cellForWall1, cellForWall2, maze.Container);
            occupiedWallsPositions.Add(wall.transform.position);

            MazeButton button = SpawnMazeButton(cellForButton, maze.Container);

            secondaryBranches.Remove(branch);

            button.OnClick.AddListener(wall.Open);
            wall.OnOpened += delegate { button.OnClick.RemoveListener(wall.Open); };
        }
    }

    private OpeningWall SpawnOpeningWall(MazeCell cell1, MazeCell cell2, Transform container)
    {
        Vector3 position = (cell1.Position + cell2.Position) / 2;
        float angle = Vector3.Angle(Vector3.up, cell2.Position - cell1.Position);
        GameObject wallGO = GameObject.Instantiate(_openingWallPrefab.gameObject, position,
            Quaternion.Euler(0, 0, angle), container);

        return wallGO.GetComponent<OpeningWall>();
    }

    private MazeButton SpawnMazeButton(MazeCell cell, Transform container)
    {
        SimpleWall connectedWall = cell.ContactWalls[Random.Range(0, cell.ContactWalls.Count)];
        float angle = Vector3.Angle(Vector3.down, connectedWall.transform.position - cell.Position);

        GameObject buttonGO = GameObject.Instantiate(_buttonPrefab.gameObject, connectedWall.transform.position,
            Quaternion.Euler(0, 0, connectedWall.transform.position.x > cell.Position.x ? angle : -angle), container);

        MazeButton button = buttonGO.GetComponent<MazeButton>();
        cell.StoredObject = button;

        return button;
    }
}
