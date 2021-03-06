using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableVector2Int : Generatable<Vector2Int>
    {
        public override void Generate()
        {
            var x = Random.Range(int.MinValue, int.MaxValue);
            var y = Random.Range(int.MinValue, int.MaxValue);

            Value = new Vector2Int(x, y);
        }
    }
}