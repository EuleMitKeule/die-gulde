using System;
using GuldeLib.Maps;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    [Serializable]
    public class GeneratableLocation : GeneratableTypeObject<Location>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Location>();

            this.Log($"Location data generating.");

            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();
            if (Value.EntityRegistry.IsGenerated) Value.EntityRegistry.Generate();

            this.Log($"Location data generated.");
        }
    }
}