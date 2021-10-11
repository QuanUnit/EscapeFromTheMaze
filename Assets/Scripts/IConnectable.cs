using System.Collections.Generic;

public interface IConnectable
{
    List<IConnectable> ConnectedSubjects { get; }
    int ConnectionsCount { get; }

    void Connect(IConnectable subject);
    void Disconnect(IConnectable subject);
    void DisconnectAll();
}