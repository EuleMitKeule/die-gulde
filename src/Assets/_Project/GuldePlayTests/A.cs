using GuldeLib.Builders;

namespace GuldePlayTests
{
    public static class A
    {
        public static GameBuilder Game => new GameBuilder();
        public static CompanyBuilder Company => new CompanyBuilder();
        public static PlayerBuilder Player => new PlayerBuilder();
        public static RecipeBuilder Recipe => new RecipeBuilder();
        public static CityBuilder City => new CityBuilder();
        public static WorkerHomeBuilder WorkerHome => new WorkerHomeBuilder();
        public static EntityBuilder Entity => new EntityBuilder();
        public static MarketBuilder Market => new MarketBuilder();
        public static MapBuilder Map => new MapBuilder();
        public static LocationBuilder Location => new LocationBuilder();
    }

    public static class An
    {
        public static ItemBuilder Item => new ItemBuilder();
        public static ExchangeBuilder Exchange => new ExchangeBuilder();
        public static InventoryBuilder Inventory => new InventoryBuilder();
    }
}