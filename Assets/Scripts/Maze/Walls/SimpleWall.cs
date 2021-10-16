using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWall : MonoBehaviour
{
    public MazeObjectLocationType LocationType { get; private set; } = MazeObjectLocationType.Inside;

    public virtual void Initialize(MazeObjectLocationType locationType)
    {
        LocationType = locationType;
    }
}
