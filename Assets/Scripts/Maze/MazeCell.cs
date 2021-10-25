using System.Collections.Generic;
using UnityEngine;

public class MazeCell
{
    public List<MazeCell> AllNeighboringCells { get; private set; } = new List<MazeCell>();
    public List<SimpleWall> ContactWalls { get; private set; } = new List<SimpleWall>();
    public bool Visited { get; set; }
    public uint DistanceToStart { get; set; }
    public Vector3 Position { get; private set; }
    public MazeObjectLocationType LocationType { get; private set; }
    public MonoBehaviour StoredObject { get; set; }

    public MazeCell(Vector3 position, MazeObjectLocationType locationType = MazeObjectLocationType.Inside, bool visited = false)
    {
        LocationType = locationType;
        Visited = visited;
        Position = position;
    }
}
