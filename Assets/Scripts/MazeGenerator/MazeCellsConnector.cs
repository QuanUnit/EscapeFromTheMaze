using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellsConnector : IConnector<MazeWall>
{
    public Dictionary<IConnectable<MazeWall>, MazeWall> ConnectedSubjects => _connectedSubjects;
    public int MaxConnectionsCount => 4;
    public int ConnectionsCount => _connectedSubjects.Count;


    private Dictionary<IConnectable<MazeWall>, MazeWall> _connectedSubjects = new Dictionary<IConnectable<MazeWall>, MazeWall>();
    private MazeCell _owner;

    public MazeCellsConnector(MazeCell owner) => _owner = owner;

    public void Connect(IConnectable<MazeWall> connectable, MazeWall connection)
    {
        if(_connectedSubjects.ContainsKey(connectable) == false)
        {
            _connectedSubjects.Add(connectable, connection);
            connectable.Connector.Connect(_owner, connection);
        }
    }
}
