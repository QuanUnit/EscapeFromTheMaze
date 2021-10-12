using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MazeGenerator : MonoBehaviour
{
    [Header("Maze settings")]

    [SerializeField] private MazeFiller _filler;

    [SerializeField] private Vector2Int _mazeSize;
    [SerializeField] private Vector2 _mazePosition;

    [SerializeField] [Range(0.001f, 1)] private float _mazeIntegrity;

    [Header("Links")]
    [SerializeField] private Transform _mazeContainer;

    [SerializeField] private GameObject _simpleWallPrefab;
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

        List<WallInfo> mazeFrame = CreateMazeFrame(mazeCellsGrid);
        List<WallInfo> insideWalls = BypassingMazeGeneration(startCell);

        DeleteWalls(insideWalls);

        Trigger exitTrigger = CreateMazeExit(mazeCellsGrid, mazeFrame, out MazeCell last);

        Maze maze = new Maze(startCell, last, mazeCellsGrid, exitTrigger,_mazeContainer);

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

        for (int i = 0; i < _mazeSize.y; i++)
        {
            float yPosition = _mazePosition.y + _wallOffset + _wallLength * i;

            Vector3 leftSidePosition = new Vector3(_mazePosition.x, yPosition);
            Vector3 rightSidePosition = new Vector3(rightMazeBorder, yPosition);

            WallInfo leftSideWall = new WallInfo(leftSidePosition, new Vector3(0, 0, 90));
            WallInfo rightSideWall = new WallInfo(rightSidePosition, new Vector3(0, 0, 90));

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

            WallInfo bottomSideWall = new WallInfo(bottomSidePosition);
            WallInfo topSideWall = new WallInfo(topSidePosition);

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
                grid[i, j] = new MazeCell(cellPosition);
            }
        }

        ÑonnectNeighboringCells(grid);

        return grid;

        void ÑonnectNeighboringCells(MazeCell[,] grid)
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

    private List<WallInfo> BypassingMazeGeneration(MazeCell startCell)
    {
        List<WallInfo> insideWalls = new List<WallInfo>();
        Stack<MazeCell> path = new Stack<MazeCell>();

        MazeCell current = startCell;
        MazeCell next;

        path.Push(current);

        while (path.Count > 0)
        {
            if(TryMoveNext(current, out next) == true)
            {
                current.DistanceToStart = (uint)path.Count;
                path.Push(current);
                current = next;
            }
            else
            {
                current = path.Pop();
            }
        }

        return insideWalls;

        void CreateWallBetweenCells(MazeCell cell1, MazeCell cell2)
        {
            Vector3 position = (cell1.Position + cell2.Position) / 2;
            float angle = Vector3.Angle(Vector3.up, cell2.Position - cell1.Position);

            WallInfo wall = new WallInfo(position, new Vector3(0, 0, angle));

            cell1.ContactWalls.Add(wall);
            cell2.ContactWalls.Add(wall);

            insideWalls.Add(wall);
        }

        void DeleteWallBetweenCells(MazeCell cell1, MazeCell cell2)
        {
            Vector3 position = (cell1.Position + cell2.Position) / 2;
            float angle = Vector3.Angle(Vector3.up, cell2.Position - cell1.Position);

            WallInfo wall = new WallInfo(position, new Vector3(0, 0, angle));

            cell1.ContactWalls.Remove(wall);
            cell2.ContactWalls.Remove(wall);

            insideWalls.Remove(wall);
        }

        bool TryMoveNext(MazeCell current, out MazeCell next)
        {
            next = default;

            var tmp = System.Array.FindAll(current.AllNeighboringCells.ToArray(), cell => ((MazeCell)cell).Visited == false);
            var possibleNeighbors = new List<IConnectable>(tmp);

            if (possibleNeighbors.Count == 0)
                return false;

            next = (MazeCell)possibleNeighbors[Random.Range(0, possibleNeighbors.Count)];
            possibleNeighbors.Remove(next);

            if(current.Visited)
                DeleteWallBetweenCells(current, next);

            current.Visited = true;
            next.Visited = true;

            current.Connect(next);

            foreach (MazeCell neighbour in possibleNeighbors)
                CreateWallBetweenCells(current, neighbour);

            return true;
        }
    }

    private Trigger CreateMazeExit(MazeCell[,] mazeCells, List<WallInfo> frame, out MazeCell last)
    {
        MazeCell farther = mazeCells[0, 0];

        for(int i = 0; i < mazeCells.GetLength(0); i++)
        {
            if (mazeCells[i, 0].DistanceToStart > farther.DistanceToStart) 
                farther = mazeCells[i, 0];

            if (mazeCells[i, mazeCells.GetLength(1) - 1].DistanceToStart > farther.DistanceToStart) 
                farther = mazeCells[i, mazeCells.GetLength(1) - 1];
        }

        for(int i = 0; i < mazeCells.GetLength(1); i++)
        {
            if (mazeCells[0, i].DistanceToStart > farther.DistanceToStart) 
                farther = mazeCells[0, i];

            if (mazeCells[mazeCells.GetLength(0) - 1, i].DistanceToStart > farther.DistanceToStart) 
                farther = mazeCells[mazeCells.GetLength(0) - 1, i];
        }

        last = farther;

        for(int i = 0; i < frame.Count; i++)
        {
            if(farther.ContactWalls.Contains(frame[i]))
            {
                GameObject exit = Instantiate(_finishTrigger.gameObject, frame[i].Position, Quaternion.Euler(frame[i].Rotation), _mazeContainer);

                farther.ContactWalls.Remove(frame[i]);
                frame.Remove(frame[i]);
                return exit.GetComponent<Trigger>();
            }
        }

        return null;
    }

    private void DeleteWalls(List<WallInfo> insideWalls)
    {
        int count = (int)Mathf.Lerp(insideWalls.Count, 0, _mazeIntegrity);

        for (int i = 0; i < count; i++)
            insideWalls.RemoveAt(Random.Range(0, insideWalls.Count));
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

        public WallInfo(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public WallInfo(Vector3 position)
        {
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
        public Vector3 Position { get; private set; }
        public bool Visited { get; set; }
        public uint DistanceToStart { get; set; }
        
        private List<IConnectable> _connectedNeighboringCells;

        public MazeCell(Vector3 position, bool visited = false)
        {
            Visited = visited;
            Position = position;
            _connectedNeighboringCells = new List<IConnectable>();
        }

        public void AddNeighbour(MazeCell cell)
        {
            if (AllNeighboringCells.Contains(cell) == false)
                AllNeighboringCells.Add(cell);
        }

        public void RemoveNeighbour(MazeCell cell)
        {
            AllNeighboringCells.Remove(cell);
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

public class Maze
{
    public MazeGenerator.MazeCell StartCell { get; private set; }
    public MazeGenerator.MazeCell LastCell { get; private set; }
    public MazeGenerator.MazeCell[,] MazeGrid { get; private set; }
    public Trigger ExitTrigger { get; private set; }
    public Transform Container { get; private set; }
    public int Width => MazeGrid.GetLength(1);
    public int Height => MazeGrid.GetLength(0);

    public Maze(MazeGenerator.MazeCell startCell, MazeGenerator.MazeCell lastCell, MazeGenerator.MazeCell[,] grid, Trigger exitTrigger, Transform container)
    {
        Container = container;  
        LastCell = lastCell;
        ExitTrigger = exitTrigger;
        MazeGrid = grid;
        StartCell = startCell;
    }

    public Maze() { }
}
