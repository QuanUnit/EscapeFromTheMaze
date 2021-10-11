using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MazeGenerator : MonoBehaviour
{
    [Header("Maze settings")]

    [SerializeField] private Vector2Int _mazeSize;
    [SerializeField] private Vector2 _mazePosition;

    [SerializeField] [Range(0.001f, 1)] private float _mazeIntegrity;

    [Header("Links")]
    [SerializeField] private Transform _mazeContainer;

    [SerializeField] private GameObject _simpleWallPrefab;
    [SerializeField] private GameObject _openingWallPrefab;
    [SerializeField] private GameObject _buttonPrefab;
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

        Trigger exitTrigger = CreateMazeExit(mazeCellsGrid, mazeFrame);

        Maze maze = new Maze(startCell, mazeCellsGrid, exitTrigger);

        BuildMaze(mazeFrame);
        BuildMaze(insideWalls);

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

            grid[_mazeSize.y - 1 - i, 0].ContactWalls.Add(leftSideWall);
            grid[_mazeSize.y - 1 - i, _mazeSize.x - 1].ContactWalls.Add(rightSideWall);

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

            grid[_mazeSize.y - 1, i].ContactWalls.Add(bottomSideWall);
            grid[0, i].ContactWalls.Add(topSideWall);

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
                        grid[i, j].Connect(neighbour);
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

        void CreateWallBetweenCells(MazeCell cell1, MazeCell cell2, WallInfo.WallType type)
        {
            Vector3 position = (cell1.Position + cell2.Position) / 2;
            float angle = Vector3.Angle(Vector3.up, cell2.Position - cell1.Position);

            WallInfo wall = new WallInfo(position, new Vector3(0, 0, angle), type);

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

            var tmp = System.Array.FindAll(current.ConnectedSubjects.ToArray(), cell => ((MazeCell)cell).Visited == false);
            var possibleNeighbors = new List<IConnectable>(tmp);

            if (possibleNeighbors.Count == 0)
                return false;

            next = (MazeCell)possibleNeighbors[Random.Range(0, possibleNeighbors.Count)];
            possibleNeighbors.Remove(next);

            if(current.Visited)
                DeleteWallBetweenCells(current, next);

            current.Visited = true;
            next.Visited = true;

            foreach (MazeCell neighbour in possibleNeighbors)
                CreateWallBetweenCells(current, neighbour, WallInfo.WallType.simple);

            return true;
        }
    }

    private Trigger CreateMazeExit(MazeCell[,] mazeCells, List<WallInfo> frame)
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
        public WallType Type => _type;

        private WallType _type;

        public WallInfo(Vector3 position, Vector3 rotation, WallType type = WallType.simple)
        {
            _type = type;
            Position = position;
            Rotation = rotation;
        }

        public WallInfo(Vector3 position, WallType type = WallType.simple)
        {
            _type = type;
            Position = position;
            Rotation = Vector3.zero;
        }

        public enum WallType
        {
            simple,
            opening,
        }
    }

    public class MazeCell : IConnectable
    {
        public List<IConnectable> ConnectedSubjects => _connectedSubjects;
        public int ConnectionsCount => _connectedSubjects.Count;
        public Vector3 Position { get; private set; }
        public bool Visited { get; set; }
        public uint DistanceToStart { get; set; }
        public List<WallInfo> ContactWalls { get; set; } = new List<WallInfo>();
        
        private List<IConnectable> _connectedSubjects;

        public MazeCell(Vector3 position, bool visited = false)
        {
            Visited = visited;
            Position = position;
            _connectedSubjects = new List<IConnectable>();
        }

        public void Connect(IConnectable subject)
        {
            if(_connectedSubjects.Contains(subject) == false)
            {
                _connectedSubjects.Add(subject);
                subject.Connect(this);
            }
        }

        public void Disconnect(IConnectable subject)
        {
            if(_connectedSubjects.Contains(subject) == true)
            {
                _connectedSubjects.Remove(subject);
                subject.Disconnect(this);   
            }
        }
        public void DisconnectAll()
        {
            foreach (var subject in _connectedSubjects.ToArray())
                Disconnect(subject);
        }
    }
}

public class Maze
{
    public MazeGenerator.MazeCell StartCell { get; private set; }
    public MazeGenerator.MazeCell[,] MazeGrid { get; private set; }
    public Trigger ExitTrigger { get; private set; }

    public Maze(MazeGenerator.MazeCell startCell, MazeGenerator.MazeCell[,] grid, Trigger exitTrigger)
    {
        ExitTrigger = exitTrigger;
        MazeGrid = grid;
        StartCell = startCell;
    }

    public Maze() { }
}
