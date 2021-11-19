using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGame.Tools
{
    public static class Tools
    {
        public static List<T> GetNeighbours<T>(this T[,] array, int i, int j)
        {
            List<T> output = new List<T>();

            Vector2Int[] directions = { new Vector2Int(1, 0), new Vector2Int(0, 1),
                                    new Vector2Int(-1, 0), new Vector2Int(0, -1) };

            foreach (var direction in directions)
            {
                Vector2Int neighboursIndx = new Vector2Int(direction.x + i, direction.y + j);

                if (neighboursIndx.x >= 0 && j + neighboursIndx.y >= 0 &&
                    neighboursIndx.x < array.GetLength(0) && neighboursIndx.y < array.GetLength(1)
                    && array[neighboursIndx.x, neighboursIndx.y] != null)
                    output.Add(array[neighboursIndx.x, neighboursIndx.y]);
            }
            return output;
        }

        public static bool Contains<T>(this IEnumerable<T> collection, T obj)
        {
            foreach (var item in collection)
            {
                if (item.Equals(obj))
                    return true;
            }
            return false;
        }

        public static T GetObject<T>(this IEnumerable<T> collection, int index)
        {
            int counter = 0;
            foreach (var item in collection)
            {
                if (index == counter)
                    return item;
                counter++;
            }

            throw new System.IndexOutOfRangeException();
        }

        public static void Invoke(MonoBehaviour owner, float delay, Action callBack)
        {
            owner.StartCoroutine(DelayProcess());

            IEnumerator DelayProcess()
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }
    }
}