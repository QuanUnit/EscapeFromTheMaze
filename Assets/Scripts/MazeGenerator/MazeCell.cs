using UnityEngine;

public class MazeCell : IConnectable<MazeWall>
{
    public IConnector<MazeWall> Connector => _connector;

    private IConnector<MazeWall> _connector;


    public MazeCell()
    {
        _connector = new MazeCellsConnector(this);
    }
}