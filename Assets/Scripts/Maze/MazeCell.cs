using System.Collections.Generic;
using UnityEngine;

public class MazeCell : IConnectable
{
    public List<IConnectable> ConnectedSubjects => _connectedNeighboringCells;
    public List<MazeCell> AllNeighboringCells { get; private set; } = new List<MazeCell>();
    public List<SimpleWall> ContactWalls { get; private set; } = new List<SimpleWall>();
    public int ConnectionsCount => _connectedNeighboringCells.Count;
    public bool Visited { get; set; }
    public uint DistanceToStart { get; set; }
    public Vector3 Position { get; private set; }
    public MazeObjectLocationType LocationType { get; private set; }

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
        if (_connectedNeighboringCells.Contains(subject) == false)
        {
            _connectedNeighboringCells.Add(subject);
            subject.Connect(this);
        }
    }

    public void Disconnect(IConnectable subject)
    {
        if (_connectedNeighboringCells.Contains(subject) == true)
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
