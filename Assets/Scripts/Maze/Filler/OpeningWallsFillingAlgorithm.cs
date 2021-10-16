using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OpeningWallsFillingAlgorithm : MazeFillingAlgorithm
{
    [SerializeField] private OpeningWall _openingWallPrefab;
    [SerializeField] private MazeButton _buttonPrefab;

    [SerializeField][Range(0, 1)] private float _wallsCountProportion;

    public override void FillMaze(Maze maze)
    {
        List<MazeCell> mainBranch = new List<MazeCell>();
        List<List<MazeCell>> secondaryBranches = new List<List<MazeCell>>();
        List<Vector3> occupiedWallsPositions = new List<Vector3>();

        foreach (var branch in maze.Branches)
        {
            if (branch.Value == Maze.BranchType.Main) mainBranch = branch.Key;
            else secondaryBranches.Add(branch.Key);
        }

        for (int i = 0; i < secondaryBranches.Count * _wallsCountProportion; i++)
        {
            List<MazeCell> branch = secondaryBranches[Random.Range(0, secondaryBranches.Count)];

            MazeCell cellForButton = branch[0];

            int cell1Index = (int)branch[branch.Count - 1].DistanceToStart;
            int cell2Index = cell1Index + 1;

            if(cell2Index >= mainBranch.Count)
            {
                secondaryBranches.Remove(branch);
                continue;
            }    

            MazeCell cellForWall1 = mainBranch[cell1Index];
            MazeCell cellForWall2 = mainBranch[cell2Index];

            if(occupiedWallsPositions.Contains((cellForWall1.Position + cellForWall2.Position) / 2))
            {
                secondaryBranches.Remove(branch);
                continue;
            }

            OpeningWall wall = SpawnOpeningWall(cellForWall1, cellForWall2, maze.Container);
            occupiedWallsPositions.Add(wall.transform.position);

            MazeButton button = SpawnMazeButton(cellForButton, maze.Container);

            secondaryBranches.Remove(branch);

            button.OnClick += wall.Open;
            wall.OnOpened += delegate { button.OnClick -= wall.Open; };
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
        float angle = Vector3.Angle(Vector3.up, connectedWall.transform.position - cell.Position);

        GameObject buttonGO = GameObject.Instantiate(_buttonPrefab.gameObject, cell.Position,
            Quaternion.Euler(0, 0, connectedWall.transform.position.x > cell.Position.x ? -angle : angle), container);

        buttonGO.transform.LookAt(cell.Position);

        MazeButton button = buttonGO.GetComponent<MazeButton>();

        return button;
    }
}
