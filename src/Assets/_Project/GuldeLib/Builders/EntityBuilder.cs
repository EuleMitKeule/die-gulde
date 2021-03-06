using GuldeLib.Entities;
using GuldeLib.Generators;
using GuldeLib.Names;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class EntityBuilder : Builder<Entity>
    {
        public EntityBuilder WithNaming(GeneratableNaming naming)
        {
            Object.Naming = naming;
            return this;
        }
    }
}