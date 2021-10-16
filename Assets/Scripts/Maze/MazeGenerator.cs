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

    [SerializeField] private GameObject _simpleWallPrefab;
    [SerializeField] private Trigger _finishTrigger;

    private float _wallLength;
    private float _wallOffset;

    [SerializeField] private GameObject test;

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

        List<WallInfo> mazeFrame = CreateMazeFrame(mazeCellsGrid);
        List<WallInfo> insideWalls = BypassingMazeGeneration(startCell, out lastCell, out var branches);

        Trigger exitTrigger = CreateMazeExit(lastCell, mazeFrame);

        Maze maze = new Maze(startCell, lastCell, mazeCellsGrid, exitTrigger, _mazeContainer, branches);

        BuildMaze(mazeFrame);
        BuildMaze(insideWalls);

        _filler.FillMaze(maze);
        
        return maze;
    }

    private void ClearContainer()
    {
        foreach (Transform child in _mazeContainer)
            Destroy(child.gameObject);
    }

    private List<WallInfo> CreateMazeFrame(MazeCell[,] grid)
    {
        List<WallInfo> frame = new List<WallInfo>();

        float topMazeBorder = _mazePosition.y + _mazeSize.y * _wallLength;
        float rightMazeBorder = _mazePosition.x + _mazeSize.x * _wallLength;
        
        MazeObjectLocationType locationType = MazeObjectLocationType.Side;
        
        for (int i = 0; i < _mazeSize.y; i++)
        {
            float yPosition = _mazePosition.y + _wallOffset + _wallLength * i;

            Vector3 leftSidePosition = new Vector3(_mazePosition.x, yPosition);
            Vector3 rightSidePosition = new Vector3(rightMazeBorder, yPosition);

            WallInfo leftSideWall = new WallInfo(leftSidePosition, new Vector3(0, 0, 90), locationType);
            WallInfo rightSideWall = new WallInfo(rightSidePosition, new Vector3(0, 0, 90), locationType);

            grid[i, 0].ContactWalls.Add(leftSideWall);
            grid[i, _mazeSize.x - 1].ContactWalls.Add(rightSideWall);

            frame.Add(leftSideWall);
            frame.Add(rightSideWall);
        }

        for (int i = 0; i < _mazeSize.x; i++)
        {
            float xPosition = _mazePosition.x + _wallOffset +_wallLength * i;

            Vector3 bottomSidePosition = new Vector3(xPosition, _mazePosition.y);
            Vector3 topSidePosition = new Vector3(xPosition, topMazeBorder);

            WallInfo bottomSideWall = new WallInfo(bottomSidePosition, locationType);
            WallInfo topSideWall = new WallInfo(topSidePosition, locationType);

            grid[0, i].ContactWalls.Add(bottomSideWall);
            grid[_mazeSize.y - 1, i].ContactWalls.Add(topSideWall);

            frame.Add(bottomSideWall);
            frame.Add(topSideWall);
        }

        return frame;
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

    private List<WallInfo> BypassingMazeGeneration(MazeCell startCell, out MazeCell lastCell, out Dictionary<List<MazeCell>, Maze.BranchType> branches)
    {
        branches = new Dictionary<List<MazeCell>, Maze.BranchType>();

        List<MazeCell> mainBranch = new List<MazeCell>();
        List<MazeCell> secondaryBranch = new List<MazeCell>();
        
        List<WallInfo> insideWalls = new List<WallInfo>();
        Stack<MazeCell> path = new Stack<MazeCell>();

        lastCell = startCell;
        
        MazeCell current = startCell;
        
        path.Push(current);
        current.Visited = true;
        
        while (path.Count > 0)
        {
            if(TryMoveNext(current, out var next) == true)
            {
                path.Push(current);
                next.DistanceToStart = current.DistanceToStart + 1;

                if (next.LocationType == MazeObjectLocationType.Side &&
                    next.DistanceToStart > lastCell.DistanceToStart)
                {
                    lastCell = next;
                    mainBranch = new List<MazeCell>(path);
                }

                if (secondaryBranch.Count != 0)
                {
                    branches.Add(new List<MazeCell>(secondaryBranch), Maze.BranchType.Secondary);
                    secondaryBranch.Clear();
                }
                
                current = next;
            }
            else
            {
                current = path.Pop();
                secondaryBranch.Add(current);
            }
        }
        
        mainBranch.Reverse();
        branches.Add(mainBranch, Maze.BranchType.Main);
        RemoveBranchesCollisions(mainBranch, branches);
        
        return insideWalls;

        void CreateWallBetweenCells(MazeCell cell1, MazeCell cell2)
        {
            Vector3 position = (cell1.Position + cell2.Position) / 2;
            float angle = Vector3.Angle(Vector3.up, cell2.Position - cell1.Position);

            WallInfo wall = new WallInfo(position, new Vector3(0, 0, angle));
            
            if(cell1.ContactWalls.Contains(wall) || cell2.ContactWalls.Contains(wall))
                return;
            
            cell1.ContactWalls.Add(wall);
            cell2.ContactWalls.Add(wall);

            insideWalls.Add(wall);
        }

        void DeleteWallBetweenCells(MazeCell cell1, MazeCell cell2)
        {
            for (int i = 0; i < cell1.ContactWalls.Count; i++)
            {
                for (int j = 0; j < cell2.ContactWalls.Count; j++)
                {
                    if (cell1.ContactWalls[i].Equals(cell2.ContactWalls[j]))
                    {
                        WallInfo wall = cell1.ContactWalls[i];
                        cell1.ContactWalls.Remove(wall);
                        cell2.ContactWalls.Remove(wall);

                        insideWalls.Remove(wall);
                        
                        return;
                    }
                }
            }
        }

        bool TryMoveNext(MazeCell current, out MazeCell next)
        {
            next = default;
            
            var possibleNeighbors = System.Array.FindAll(current.AllNeighboringCells.ToArray(), cell => cell.Visited == false);

            if (possibleNeighbors.Length == 0)
                return false;

            next = possibleNeighbors[Random.Range(0, possibleNeighbors.Length)];

            if(current.Visited == true)
                DeleteWallBetweenCells(current, next);

            next.Visited = true;
            
            current.Connect(next);

            foreach (MazeCell neighbour in possibleNeighbors)
                if(neighbour != next) CreateWallBetweenCells(current, neighbour);

            return true;
        }
    }
    
    void RemoveBranchesCollisions(List<MazeCell> mask, Dictionary<List<MazeCell>, Maze.BranchType> mazeBranches)
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

    private Trigger CreateMazeExit(MazeCell lastSideCell, List<WallInfo> mazeFrame)
    {
        for(int i = 0; i < lastSideCell.ContactWalls.Count; i++)
        {
            WallInfo sideWall = lastSideCell.ContactWalls[i];
            
            if(sideWall.LocationType == MazeObjectLocationType.Side)
            {
                GameObject exit = Instantiate(_finishTrigger.gameObject, sideWall.Position, Quaternion.Euler(sideWall.Rotation), _mazeContainer);

                lastSideCell.ContactWalls.Remove(sideWall);
                mazeFrame.Remove(sideWall);
                
                return exit.GetComponent<Trigger>();
            }
        }

        return null;
    }

    private void BuildMaze(List<WallInfo> walls)
    {
        foreach (var wall in walls)
            Instantiate(_simpleWallPrefab, wall.Position, Quaternion.Euler(wall.Rotation), _mazeContainer);
    }

    public struct WallInfo
    {
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public MazeObjectLocationType LocationType { get; private set; }

        public WallInfo(Vector3 position, Vector3 rotation, MazeObjectLocationType locationType = MazeObjectLocationType.Inside)
        {
            LocationType = locationType;
            Position = position;
            Rotation = rotation;
        }

        public WallInfo(Vector3 position, MazeObjectLocationType locationType = MazeObjectLocationType.Inside)
        {
            LocationType = locationType;
            Position = position;
            Rotation = Vector3.zero;
        }
    }

    public class MazeCell : IConnectable
    {
        public List<IConnectable> ConnectedSubjects => _connectedNeighboringCells;
        public List<MazeCell> AllNeighboringCells { get; private set; } = new List<MazeCell>();
        public List<WallInfo> ContactWalls { get; private set; } = new List<WallInfo>();
        public int ConnectionsCount => _connectedNeighboringCells.Count;
        public bool Visited { get; set; }
        public uint DistanceToStart { get; set; }
        public Vector3 Position { get; private set; }
        public  MazeObjectLocationType LocationType { get; private set; } 
        
        private List<IConnectable> _connectedNeighboringCells;

        public MazeCell(Vector3 position, MazeObjectLocationType locationType = MazeObjectLocationType.Inside, bool visited = false)
        {
            LocationType = locationType;
            Visited = visited;
            Position = position;
            _connectedNeighboringCells = new List<IConnectable>();
        }

        public void Connect(IConnectable subject)
        {
            if(_connectedNeighboringCells.Contains(subject) == false)
            {
                _connectedNeighboringCells.Add(subject);
                subject.Connect(this);
            }
        }

        public void Disconnect(IConnectable subject)
        {
            if(_connectedNeighboringCells.Contains(subject) == true)
            {
                _connectedNeighboringCells.Remove(subject);
                subject.Disconnect(this);   
            }
        }
        public void DisconnectAll()
        {
            foreach (var subject in _connectedNeighboringCells.ToArray())
                Disconnect(subject);
        }
    }
}

public enum MazeObjectLocationType
{
    Side,
    Inside,
}

public class Maze
{
    public MazeGenerator.MazeCell StartCell { get; private set; }
    public MazeGenerator.MazeCell LastCell { get; private set; }
    public MazeGenerator.MazeCell[,] MazeGrid { get; private set; }
    public Dictionary<List<MazeGenerator.MazeCell>, BranchType> Branches { get; private set; }
    public Trigger ExitTrigger { get; private set; }
    public Transform Container { get; private set; }
    
    public Maze(MazeGenerator.MazeCell startCell, MazeGenerator.MazeCell lastCell, MazeGenerator.MazeCell[,] grid, 
        Trigger exitTrigger, Transform container, Dictionary<List<MazeGenerator.MazeCell>, BranchType> branches)
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
