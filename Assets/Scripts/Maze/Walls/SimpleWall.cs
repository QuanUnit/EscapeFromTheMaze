using UnityEngine;

namespace MazeGame.Maze.Environment
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleWall : MonoBehaviour
    {
        public MazeObjectLocationType LocationType { get; private set; } = MazeObjectLocationType.Inside;

        public virtual void Initialize(MazeObjectLocationType locationType)
        {
            LocationType = locationType;
        }
    }

}