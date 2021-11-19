namespace MazeGame.Abstract
{
    public interface IPropertyChangeNotifier
    {
        event System.Action<object> PropertyOnChanged;
    }
}