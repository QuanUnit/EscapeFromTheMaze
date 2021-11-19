using System.Collections.Generic;

namespace MazeGame.Abstract
{
    public interface IConnectable
    {
        List<IConnectable> ConnectedSubjects { get; }
        int ConnectionsCount { get; }

        void Connect(IConnectable subject);
        void Disconnect(IConnectable subject);
        void DisconnectAll();
    }
}