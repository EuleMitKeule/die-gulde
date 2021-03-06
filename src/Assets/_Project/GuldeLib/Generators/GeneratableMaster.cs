using GuldeLib.Companies;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableMaster : GeneratableTypeObject<Master>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Master>();

            this.Log($"Master data generating.");

            if (Value.ProductionAgent.IsGenerated) Value.ProductionAgent.Generate();

            this.Log($"Master data generated.");
        }
    }
}