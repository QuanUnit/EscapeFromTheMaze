public interface IConnectable<T>
{
    IConnector<T> Connector { get; }
}