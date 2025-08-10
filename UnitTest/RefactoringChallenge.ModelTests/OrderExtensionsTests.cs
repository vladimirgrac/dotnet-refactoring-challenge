using FluentAssertions;
using RefactoringChallenge.Model.Domain;
using RefactoringChallenge.Model.Extensions;

namespace RefactoringChallenge.Tests
{
    public class OrderExtensionsTests
    {
        private Order CreateOrder(decimal unitPrice, int quantity)
        {
            return new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { UnitPrice = unitPrice, Quantity = quantity }
                }
            };
        }

        [Fact]
        public void ProcessOrder_VipOldCustomerLargeOrder_AppliesMaxDiscount25Percent()
        {
            // Arrange
            var order = CreateOrder(3000m, 5); // total = 15000
            bool isVip = true;
            int registrationYear = DateTime.Now.Year - 10;

            // Act
            order.ProcessOrder(isVip, registrationYear);

            // Assert
            order.DiscountPercent.Should().Be(25);
            order.TotalAmount.Should().Be(15000m - (15000m * 0.25m));
            order.Status.Should().Be("Processed");
        }

        [Fact]
        public void ProcessOrder_RegularCustomer2YearsLargeOrder_Applies12PercentDiscount()
        {
            // Arrange
            var order = CreateOrder(3000m, 3); // total = 9000
            bool isVip = false;
            int registrationYear = DateTime.Now.Year - 2;

            // Act
            order.ProcessOrder(isVip, registrationYear);

            // Assert
            order.DiscountPercent.Should().Be(12); // 2 loyalty + 10 total >5000
            order.TotalAmount.Should().Be(9000m - (9000m * 0.12m));
        }

        [Fact]
        public void ProcessOrder_VipRecentCustomerSmallOrder_Applies15PercentDiscount()
        {
            // Arrange
            var order = CreateOrder(300m, 4); // total = 1200
            bool isVip = true;
            int registrationYear = DateTime.Now.Year;

            // Act
            order.ProcessOrder(isVip, registrationYear);

            // Assert
            order.DiscountPercent.Should().Be(15); // 10 VIP + 5 total >1000
            order.TotalAmount.Should().Be(1200m - (1200m * 0.15m));
        }

        [Fact]
        public void ProcessOrder_RegularCustomerOldSmallOrder_Applies5PercentLoyaltyDiscountOnly()
        {
            // Arrange
            var order = CreateOrder(100m, 2); // total = 200
            bool isVip = false;
            int registrationYear = DateTime.Now.Year - 5;

            // Act
            order.ProcessOrder(isVip, registrationYear);

            // Assert
            order.DiscountPercent.Should().Be(5);
            order.TotalAmount.Should().Be(200m - (200m * 0.05m));
        }

        [Fact]
        public void ProcessOrder_ZeroItems_SetsAllAmountsToZero()
        {
            // Arrange
            var order = new Order { Items = new List<OrderItem>() };
            bool isVip = false;
            int registrationYear = DateTime.Now.Year;

            // Act
            order.ProcessOrder(isVip, registrationYear);

            // Assert
            order.TotalAmount.Should().Be(0);
            order.DiscountAmount.Should().Be(0);
            order.DiscountPercent.Should().Be(0);
        }
    }
}
