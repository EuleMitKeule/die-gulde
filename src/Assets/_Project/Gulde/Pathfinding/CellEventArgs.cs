using System;
using UnityEngine;

namespace Gulde.Pathfinding
{
    public class CellEventArgs : EventArgs
    {
        Vector3Int Cell { get; }

        public CellEventArgs(Vector3Int cell) => Cell = cell;
    }
}