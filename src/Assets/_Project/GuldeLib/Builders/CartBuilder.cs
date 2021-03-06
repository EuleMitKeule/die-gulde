using GuldeLib.Companies.Carts;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Generators;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class CartBuilder : Builder<Cart>
    {
        public CartBuilder WithCartType(CartComponent.CartType cartType)
        {
            Object.CartType = cartType;
            return this;
        }

        public CartBuilder WithTravel(GeneratableTravel travel)
        {
            Object.Travel = travel;
            return this;
        }

        public CartBuilder WithExchange(GeneratableExchange exchange)
        {
            Object.Exchange = exchange;
            return this;
        }
    }
}