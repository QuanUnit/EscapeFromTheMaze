using System.Collections.Generic;

public interface IConnector<T>
{
    Dictionary<IConnectable<T>, T> ConnectedSubjects { get; }
    void Connect(IConnectable<T> connectable, T connection);
    int MaxConnectionsCount { get; }
    int ConnectionsCount { get; }
}
