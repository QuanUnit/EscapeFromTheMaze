using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleWall : MonoBehaviour
{
    public MazeObjectLocationType LocationType { get; private set; } = MazeObjectLocationType.Inside;

    public virtual void Initialize(MazeObjectLocationType locationType)
    {
        LocationType = locationType;
    }
}
