using System.Collections.Generic;
using MazeGame.Abstract;
using MazeGame.Maze.Environment;
using UnityEngine;

namespace MazeGame.Maze
{
    public class MazeCell : IConnectable
    {
        public List<MazeCell> AllNeighboringCells { get; private set; } = new List<MazeCell>();
        public List<SimpleWall> ContactWalls { get; private set; } = new List<SimpleWall>();
        public bool Visited { get; set; }
        public uint DistanceToStart { get; set; }
        public Vector3 Position { get; private set; }
        public MazeObjectLocationType LocationType { get; private set; }
        public List<MonoBehaviour> StoredObjects { get; set; } = new List<MonoBehaviour>();

        public List<IConnectable> ConnectedSubjects => _connectedSubjects;
        public int ConnectionsCount => _connectedSubjects.Count;

        private List<IConnectable> _connectedSubjects = new List<IConnectable>();

        public MazeCell(Vector3 position, MazeObjectLocationType locationType = MazeObjectLocationType.Inside, bool visited = false)
        {
            LocationType = locationType;
            Visited = visited;
            Position = position;
        }

        public void Connect(IConnectable subject)
        {
            if (_connectedSubjects.Contains(subject) == false)
            {
                _connectedSubjects.Add((MazeCell)subject);
                subject.Connect(this);
            }
        }

        public void Disconnect(IConnectable subject)
        {
            if (_connectedSubjects.Contains(subject) == true)
            {
                _connectedSubjects.Remove((MazeCell)subject);
                subject.Disconnect(this);
            }
        }

        public void DisconnectAll()
        {
            while (_connectedSubjects.Count > 0)
                Disconnect(_connectedSubjects[0]);
        }
    }
}