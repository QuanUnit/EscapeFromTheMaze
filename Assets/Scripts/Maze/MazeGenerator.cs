using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public sealed class MazeGenerator : MonoBehaviour
{
    [Header("Maze settings")]

    [SerializeField] private MazeFiller _filler;

    [SerializeField] private Vector2Int _mazeSize;
    [SerializeField] private Vector2 _mazePosition;

    [Header("Links")]
    [SerializeField] private Transform _mazeContainer;

    [SerializeField] private SimpleWall _simpleWallPrefab;
    [SerializeField] private Trigger _finishTrigger;

    private float _wallLength;
    private float _wallOffset;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        Sprite wallSprite = _simpleWallPrefab.GetComponent<SpriteRenderer>().sprite;

        Vector2 wallSpriteSize = new Vector2(wallSprite.texture.width, wallSprite.texture.height);
        Vector2 wallSize = (wallSpriteSize / wallSprite.pixelsPerUnit * _simpleWallPrefab.transform.localScale);

        _wallLength = wallSize.x - wallSize.y;
        _wallOffset = (wallSprite.pivot / wallSpriteSize).x * _wallLength;
    }

    public Maze Generate()
    {
        ClearContainer();

        MazeCell[,] mazeCellsGrid = GenerateCellsGrid();
        MazeCell startCell = mazeCellsGrid[Random.Range(0, _mazeSize.y), Random.Range(0, _mazeSize.x)];
        MazeCell lastCell;

        CreateMazeFrame(mazeCellsGrid);
        BypassingMazeGeneration(startCell, out lastCell, out var branches);

        Trigger exitTrigger = CreateMazeExit(lastCell);

        Maze maze = new Maze(startCell, lastCell, mazeCellsGrid, exitTrigger, _mazeContainer, branches);

        _filler.FillMaze(maze);

        return maze;
    }

    private void ClearContainer()
    {
        foreach (Transform child in _mazeContainer)
            Destroy(child.gameObject);
    }

    private void CreateMazeFrame(MazeCell[,] grid)
    {
        float topMazeBorder = _mazePosition.y + _mazeSize.y * _wallLength;
        float rightMazeBorder = _mazePosition.x + _mazeSize.x * _wallLength;

        Quaternion verticalRotation = Quaternion.Euler(0, 0, 90);
        Quaternion horizontalRotation = Quaternion.Euler(0, 0, 0);

        for (int i = 0; i < _mazeSize.y; i++)
        {
            float yPosition = _mazePosition.y + _wallOffset + _wallLength * i;

            Vector3 leftSidePosition = new Vector3(_mazePosition.x, yPosition);
            Vector3 rightSidePosition = new Vector3(rightMazeBorder, yPosition);

            SimpleWall leftSideWall = CreateWall(leftSidePosition, verticalRotation, MazeObjectLocationType.Side, _mazeContainer);
            SimpleWall rightSideWall = CreateWall(rightSidePosition, verticalRotation, MazeObjectLocationType.Side, _mazeContainer);

            grid[i, 0].ContactWalls.Add(leftSideWall);
            grid[i, _mazeSize.x - 1].ContactWalls.Add(rightSideWall);

        }

        for (int i = 0; i < _mazeSize.x; i++)
        {
            float xPosition = _mazePosition.x + _wallOffset +_wallLength * i;

            Vector3 bottomSidePosition = new Vector3(xPosition, _mazePosition.y);
            Vector3 topSidePosition = new Vector3(xPosition, topMazeBorder);

            SimpleWall bottomSideWall = CreateWall(bottomSidePosition, horizontalRotation, MazeObjectLocationType.Side, _mazeContainer);
            SimpleWall topSideWall = CreateWall(topSidePosition, horizontalRotation, MazeObjectLocationType.Side, _mazeContainer);

            grid[0, i].ContactWalls.Add(bottomSideWall);
            grid[_mazeSize.y - 1, i].ContactWalls.Add(topSideWall);

        }
    }

    private MazeCell[,] GenerateCellsGrid()
    {
        MazeCell[,] grid = new MazeCell[_mazeSize.y, _mazeSize.x];

        Vector3 startPosition = new Vector3(_mazePosition.x + _wallLength / 2, _mazePosition.y + _wallLength / 2);

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Vector3 cellPosition = startPosition + new Vector3(_wallLength * j, _wallLength * i);
                MazeObjectLocationType locationType = MazeObjectLocationType.Inside;

                if (i == 0 || i == grid.GetLength(0) - 1 ||
                    j == 0 || j == grid.GetLength(1) - 1)
                    locationType = MazeObjectLocationType.Side;
                
                grid[i, j] = new MazeCell(cellPosition, locationType);
            }
        }

        ConnectNeighboringCells(grid);

        return grid;

        void ConnectNeighboringCells(MazeCell[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    List<MazeCell> neighboringCells = grid.GetNeighbours(i, j);

                    foreach (var neighbour in neighboringCells)
                        grid[i, j].AllNeighboringCells.Add(neighbour);
                }
            }
        }
    }

    private void BypassingMazeGeneration(MazeCell startCell, out MazeCell lastCell, out Dictionary<List<MazeCell>, Maze.BranchType> branches)
    {
        branches = new Dictionary<List<MazeCell>, Maze.BranchType>();

        List<MazeCell> mainBranch = new List<MazeCell>();
        List<MazeCell> secondaryBranch = new List<MazeCell>();
        
        Stack<MazeCell> path = new Stack<MazeCell>();
        
        MazeCell current = startCell;
        lastCell = startCell;

        path.Push(current);
        current.Visited = true;

        while (path.Count > 0)
        {
            bool canMoveNext = CanMoveNext(current, out var next, out var freeNeighbors);

            if (canMoveNext == true)
            {
                TryRemoveWallBetweenCells(current, next);

                path.Push(current);
                next.DistanceToStart = current.DistanceToStart + 1;

                if (current.LocationType == MazeObjectLocationType.Side &&
                     current.DistanceToStart > lastCell.DistanceToStart)
                {
                    lastCell = current;
                    mainBranch = new List<MazeCell>(path);
                }

                if (secondaryBranch.Count != 0)
                {
                    branches.Add(new List<MazeCell>(secondaryBranch), Maze.BranchType.Secondary);
                    secondaryBranch.Clear();
                }

                foreach (var neighbour in freeNeighbors)
                {
                    if(neighbour.Visited == false) CreateWallBetweenCells(current, neighbour);
                }

                next.Visited = true;
                current = next;
            }
            else
            {
                secondaryBranch.Add(current);
                current = path.Pop();
            }
        }

        mainBranch.Reverse();
        branches.Add(mainBranch, Maze.BranchType.Main);
        RemoveBranchesCollisions(mainBranch, branches);
    }

    private bool CanMoveNext(MazeCell current, out MazeCell next, out List<MazeCell> freeNeighbors)
    {
        next = default;

        freeNeighbors = new List<MazeCell>();

        foreach (var cell in current.AllNeighboringCells)
            if (cell.Visited == false) freeNeighbors.Add(cell);

        if (freeNeighbors.Count == 0) return false;

        next = freeNeighbors[Random.Range(0, freeNeighbors.Count)];
        freeNeighbors.Remove(next);

        return true;
    }

    private void CreateWallBetweenCells(MazeCell cell1, MazeCell cell2)
    {
        Vector3 position = (cell1.Position + cell2.Position) / 2;
        float angle = Vector3.Angle(Vector3.up, cell2.Position - cell1.Position);

        SimpleWall wall = CreateWall(position, Quaternion.Euler(0, 0, angle), MazeObjectLocationType.Inside, _mazeContainer);

        cell1.ContactWalls.Add(wall);
        cell2.ContactWalls.Add(wall);
    }

    private bool TryRemoveWallBetweenCells(MazeCell cell1, MazeCell cell2)
    {
        for (int i = 0; i < cell1.ContactWalls.Count; i++)
        {
            SimpleWall wall = cell1.ContactWalls[i];

            if (cell2.ContactWalls.Contains(wall))
            {
                cell1.ContactWalls.Remove(wall);
                cell2.ContactWalls.Remove(wall);

                Destroy(wall.gameObject);
            }
        }

        return false;
    }

    private void RemoveBranchesCollisions(List<MazeCell> mask, Dictionary<List<MazeCell>, Maze.BranchType> mazeBranches)
    {
        foreach (var branch in mazeBranches)
        {
            if(branch.Value == Maze.BranchType.Main)
                continue;

            for (int i = 0; i < branch.Key.Count; i++)
            {
                if (mask.Contains(branch.Key[i]))
                {
                    branch.Key.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private Trigger CreateMazeExit(MazeCell lastSideCell)
    {
        for(int i = 0; i < lastSideCell.ContactWalls.Count; i++)
        {
            SimpleWall sideWall = lastSideCell.ContactWalls[i];
            
            if(sideWall.LocationType == MazeObjectLocationType.Side)
            {
                GameObject exit = Instantiate(_finishTrigger.gameObject, sideWall.transform.position, sideWall.transform.rotation, _mazeContainer);

                lastSideCell.ContactWalls.Remove(sideWall);
                Destroy(sideWall.gameObject);
                
                return exit.GetComponent<Trigger>();
            }
        }

        return null;
    }

    private SimpleWall CreateWall(Vector3 position, Quaternion rotation, MazeObjectLocationType locationType, Transform container)
    {
        GameObject wallGO = Instantiate(_simpleWallPrefab.gameObject, position, rotation, container);

        SimpleWall wall = wallGO.GetComponent<SimpleWall>();
        wall.Initialize(locationType);

        return wall;
    }
}

public enum MazeObjectLocationType
{
    Side,
    Inside,
}

public class Maze
{
    public MazeCell StartCell { get; private set; }
    public MazeCell LastCell { get; private set; }
    public MazeCell[,] MazeGrid { get; private set; }
    public Dictionary<List<MazeCell>, BranchType> Branches { get; private set; }
    public Trigger ExitTrigger { get; private set; }
    public Transform Container { get; private set; }
    
    public Maze(MazeCell startCell, MazeCell lastCell, MazeCell[,] grid, 
        Trigger exitTrigger, Transform container, Dictionary<List<MazeCell>, BranchType> branches)
    {
        Branches = branches;
        Container = container;  
        LastCell = lastCell;
        ExitTrigger = exitTrigger;
        MazeGrid = grid;
        StartCell = startCell;
    }

    public enum BranchType
    {
        Main,
        Secondary,
    }
}