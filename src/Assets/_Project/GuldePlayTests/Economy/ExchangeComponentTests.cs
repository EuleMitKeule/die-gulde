// using System.Collections;
// using System.Linq;
// using GuldeLib;
// using GuldeLib.Builders;
// using GuldeLib.Cities;
// using GuldeLib.Companies;
// using GuldeLib.Companies.Carts;
// using GuldeLib.Economy;
// using GuldeLib.Producing;
// using MonoLogger.Runtime;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// namespace GuldePlayTests.Economy
// {
//     public class ExchangeComponentTests
//     {
//         GameBuilder GameBuilder { get; set; }
//         CityBuilder CityBuilder { get; set; }
//         CompanyBuilder CompanyBuilder { get; set; }
//         PlayerBuilder PlayerBuilder { get; set; }
//
//         Item Resource { get; set; }
//         Item Product { get; set; }
//         GameObject PlayerObject => PlayerBuilder.PlayerObject;
//         WealthComponent Owner => PlayerObject.GetComponent<WealthComponent>();
//         GameObject CompanyObject => CompanyBuilder.CompanyObject;
//         CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
//         GameObject CityObject => CityBuilder.CityObject;
//         CityComponent City => CityObject.GetComponent<CityComponent>();
//         MarketComponent Market => City.Market;
//         ExchangeComponent MarketExchange => Market.Location.Exchanges.ElementAt(0);
//         CartComponent Cart => Company.Carts.ElementAt(0);
//
//         float BoughtPrice { get; set; }
//         int BoughtAmount { get; set; }
//         Item BoughtItem { get; set; }
//         bool ItemBoughtFlag { get; set; }
//         float SoldPrice { get; set; }
//         int SoldAmount { get; set; }
//         Item SoldItem { get; set; }
//         bool ItemSoldFlag { get; set; }
//
//         [UnitySetUp]
//         public IEnumerator Setup()
//         {
//             PlayerBuilder = A.Player;
//
//             CompanyBuilder = A.Company
//                 .WithOwner(PlayerBuilder)
//                 .WithCarts(1)
//                 .WithEntryCell(1, 1);
//
//             var marketBuilder = A.Market
//                 .WithExchange("market_exchange_1", An.Exchange);
//
//             CityBuilder = A.City
//                 .WithSize(20, 20)
//                 .WithMarket(marketBuilder)
//                 .WithCompany(CompanyBuilder)
//                 .WithNormalTimeSpeed(300);
//
//             GameBuilder = A.Game
//                 .WithCity(CityBuilder)
//                 .WithTimeScale(5f);
//
//             Resource = An.Item
//                 .WithName("resource")
//                 .WithItemType(ItemType.Resource)
//                 .WithMeanPrice(100)
//                 .WithMeanSupply(10)
//                 .WithMinPrice(50)
//                 .Build();
//             Product = An.Item
//                 .WithName("product")
//                 .WithItemType(ItemType.Product)
//                 .WithMeanPrice(100)
//                 .WithMeanSupply(10)
//                 .WithMinPrice(50)
//                 .Build();
//
//             yield return GameBuilder.Build();
//         }
//
//         [TearDown]
//         public void Teardown()
//         {
//             foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
//             {
//                 Object.DestroyImmediate(gameObject);
//             }
//
//             BoughtPrice = 0f;
//             BoughtAmount = 0;
//             BoughtItem = null;
//             ItemBoughtFlag = false;
//
//             SoldPrice = 0f;
//             SoldAmount = 0;
//             SoldItem = null;
//             ItemSoldFlag = false;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldBeAbleToTransferWithCompany()
//         {
//             Assert.True(Cart.Exchange.CanExchangeWith(Company.Exchange));
//             Assert.True(Company.Exchange.CanExchangeWith(Cart.Exchange));
//             yield break;
//         }
//
//         [Test]
//         public void ShouldNotBeAbleToTransferWithMarket()
//         {
//             Assert.False(Cart.Exchange.CanExchangeWith(MarketExchange));
//             Assert.False(MarketExchange.CanExchangeWith(Cart.Exchange));
//         }
//
//         [Test]
//         public void ShouldGetMeanPriceWhenMeanSupply()
//         {
//             MarketExchange.AddItem(Product, 10);
//             Assert.AreEqual(10, MarketExchange.Inventory.GetSupply(Product));
//             Assert.AreEqual(100f, MarketExchange.GetPrice(Product));
//         }
//
//         /// <remarks>
//         /// An empty cart travels to a market which is first not accepting sales.
//         /// The cart should not be able to sell the product.
//         /// After adding the product, the cart should still not be able to sell the product.
//         /// After making the market accepting sales and removing the product from the cart,
//         /// the cart should still not be able to sell the product.
//         /// The cart travels back to the company and the product is again removed from the cart.
//         /// The cart should still not be able to sell the product.
//         /// </remarks>
//         [UnityTest]
//         public IEnumerator ShouldNotBeAbleToSellWhenInvalid()
//         {
//             MonoLogger.Runtime.MonoLogger.DefaultLogLevel = LogType.Log;
//             Cart.Exchange.SetLogLevel(LogType.Log);
//             MarketExchange.SetLogLevel(LogType.Log);
//
//             MarketExchange.IsPurchasing = false;
//
//             Cart.Travel.TravelTo(MarketExchange.Location);
//
//             yield return Cart.Travel.WaitForDestinationReached;
//
//             Assert.True(MarketExchange.Location.EntityRegistry.IsRegistered(Cart.Entity));
//             Assert.True(Cart.Exchange.CanExchangeWith(MarketExchange));
//             Assert.False(Cart.Exchange.CanSellTo(Product, MarketExchange));
//
//             Cart.Exchange.AddItem(Product);
//
//             Cart.Exchange.Sell(Product, MarketExchange);
//
//             Assert.False(ItemSoldFlag);
//             Assert.False(ItemBoughtFlag);
//             Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Product));
//             Assert.AreEqual(0, MarketExchange.Inventory.GetSupply(Product));
//
//             MarketExchange.IsPurchasing = true;
//
//             Cart.Exchange.RemoveItem(Product);
//
//             Cart.Exchange.Sell(Product, MarketExchange);
//
//             Assert.False(ItemSoldFlag);
//             Assert.False(ItemBoughtFlag);
//             Assert.AreEqual(0, Cart.Exchange.Inventory.GetSupply(Product));
//             Assert.AreEqual(0, MarketExchange.Inventory.GetSupply(Product));
//
//             Cart.Travel.TravelTo(Company.Location);
//
//             yield return Cart.Travel.WaitForDestinationReached;
//
//             Cart.Exchange.AddItem(Product);
//             Cart.Exchange.Sell(Product, MarketExchange);
//
//             Assert.False(ItemSoldFlag);
//             Assert.False(ItemBoughtFlag);
//             Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Product));
//             Assert.AreEqual(0, MarketExchange.Inventory.GetSupply(Product));
//         }
//
//         [Test]
//         public void ShouldGetTargetInventory()
//         {
//             var resourceInventory = Company.Exchange.GetTargetInventory(Resource);
//             var productInventory = Company.Exchange.GetTargetInventory(Product);
//
//             Assert.AreEqual(Company.Exchange.Inventory, resourceInventory);
//             Assert.AreEqual(Company.Exchange.ProductInventory, productInventory);
//         }
//
//         [Test]
//         public void ShouldNotGetProductInventoryWhenMissing()
//         {
//             Assert.IsNull(Cart.Exchange.ProductInventory);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldSellItem()
//         {
//             Cart.Travel.TravelTo(Market.Location);
//
//             yield return Cart.Travel.WaitForDestinationReached;
//
//             Cart.Exchange.ItemSold += OnItemSold;
//             MarketExchange.ItemBought += OnItemBought;
//             Cart.Exchange.AddItem(Product);
//             Cart.Exchange.Sell(Product, MarketExchange);
//
//             Assert.True(ItemBoughtFlag);
//             Assert.AreEqual(Product, BoughtItem);
//             Assert.AreEqual(Product.MaxPrice, BoughtPrice);
//             Assert.AreEqual(1, BoughtAmount);
//             Assert.True(ItemSoldFlag);
//             Assert.AreEqual(Product, SoldItem);
//             Assert.AreEqual(Product.MaxPrice, SoldPrice);
//             Assert.AreEqual(1, SoldAmount);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldBuyItem()
//         {
//             Cart.Travel.TravelTo(Market.Location);
//
//             yield return Cart.Travel.WaitForDestinationReached;
//
//             MarketExchange.ItemSold += OnItemSold;
//             Cart.Exchange.ItemBought += OnItemBought;
//             MarketExchange.AddItem(Product);
//             Owner.AddMoney(145f);
//             Assert.AreEqual(145f, Owner.Money);
//             Cart.Exchange.Purchase(Product, MarketExchange);
//
//             Assert.True(ItemBoughtFlag);
//             Assert.AreEqual(Product, BoughtItem);
//             Assert.AreEqual(145f, BoughtPrice);
//             Assert.AreEqual(1, BoughtAmount);
//             Assert.True(ItemSoldFlag);
//             Assert.AreEqual(Product, SoldItem);
//             Assert.AreEqual(145f, SoldPrice);
//             Assert.AreEqual(1, SoldAmount);
//             Assert.AreEqual(0f, Owner.Money);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldNotBuyItemWhenNotInStock()
//         {
//             Cart.Travel.TravelTo(Market.Location);
//
//             yield return Cart.Travel.WaitForDestinationReached;
//
//             MarketExchange.ItemSold += OnItemSold;
//             Cart.Exchange.ItemBought += OnItemBought;
//             Cart.Company.Owner.AddMoney(150f);
//             Cart.Exchange.Purchase(Product, MarketExchange);
//
//             Assert.False(ItemBoughtFlag);
//             Assert.IsNull(BoughtItem);
//             Assert.AreEqual(0, BoughtPrice);
//             Assert.AreEqual(0, BoughtAmount);
//             Assert.False(ItemSoldFlag);
//             Assert.IsNull(SoldItem);
//             Assert.AreEqual(0, SoldPrice);
//             Assert.AreEqual(0, SoldAmount);
//             Assert.AreEqual(150f, Owner.Money);
//         }
//
//         [Test]
//         public void ShouldNotBuyItemWhenNotAtMarket()
//         {
//             MarketExchange.ItemSold += OnItemSold;
//             Cart.Exchange.ItemBought += OnItemBought;
//             MarketExchange.AddItem(Product);
//             Cart.Company.Owner.AddMoney(150f);
//             Cart.Exchange.Purchase(Product, MarketExchange);
//
//             Assert.False(ItemBoughtFlag);
//             Assert.IsNull(BoughtItem);
//             Assert.AreEqual(0, BoughtPrice);
//             Assert.AreEqual(0, BoughtAmount);
//             Assert.False(ItemSoldFlag);
//             Assert.IsNull(SoldItem);
//             Assert.AreEqual(0, SoldPrice);
//             Assert.AreEqual(0, SoldAmount);
//             Assert.AreEqual(150f, Owner.Money);
//         }
//
//         [Test]
//         public void ShouldGiveItem()
//         {
//             var companyTargetInventory = Company.Exchange.GetTargetInventory(Product);
//
//             Cart.Exchange.ItemSold += OnItemSold;
//             Company.Exchange.ItemBought += OnItemBought;
//             Cart.Exchange.AddItem(Product);
//             Cart.Exchange.Sell(Product, Company.Exchange);
//
//             Assert.AreEqual(Cart.Exchange.Owner, Company.Exchange.Owner);
//             Assert.False(ItemBoughtFlag);
//             Assert.False(ItemSoldFlag);
//             Assert.AreEqual(0, Cart.Exchange.Inventory.GetSupply(Product));
//             Assert.AreEqual(1, companyTargetInventory.GetSupply(Product));
//         }
//
//         [Test]
//         public void ShouldTakeItem()
//         {
//             Company.Exchange.ItemSold += OnItemSold;
//             Cart.Exchange.ItemBought += OnItemBought;
//             Company.Exchange.AddItem(Product);
//             Cart.Exchange.Purchase(Product, Company.Exchange);
//
//             Assert.False(ItemBoughtFlag);
//             Assert.False(ItemSoldFlag);
//             Assert.AreEqual(0, Company.Exchange.ProductInventory.GetSupply(Product));
//             Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Product));
//         }
//
//         void OnItemSold(object sender, ItemSoldEventArgs e)
//         {
//             ItemSoldFlag = true;
//             SoldItem = e.Item;
//             SoldAmount = e.Amount;
//             SoldPrice = e.Price;
//         }
//
//         void OnItemBought(object sender, ItemBoughtEventArgs e)
//         {
//             ItemBoughtFlag = true;
//             BoughtItem = e.Item;
//             BoughtAmount = e.Amount;
//             BoughtPrice = e.Price;
//         }
//     }
// }