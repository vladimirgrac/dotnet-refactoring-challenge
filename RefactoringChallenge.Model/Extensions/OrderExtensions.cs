using RefactoringChallenge.Model.Domain;

namespace RefactoringChallenge.Model.Extensions;
public static class OrderExtensions
{
    public static void ProcessOrder(this Order order, bool isVip, int registrationYear)
    {
        decimal totalAmount = 0;
        foreach (var item in order.Items)
        {
            var subtotal = item.Quantity * item.UnitPrice;
            totalAmount += subtotal;
        }

        decimal discountPercent = 0;

        if (isVip)
        {
            discountPercent += 10;
        }

        int yearsAsCustomer = DateTime.Now.Year - registrationYear;
        if (yearsAsCustomer >= 5)
        {
            discountPercent += 5;
        }
        else if (yearsAsCustomer >= 2)
        {
            discountPercent += 2;
        }

        if (totalAmount > 10000)
        {
            discountPercent += 15;
        }
        else if (totalAmount > 5000)
        {
            discountPercent += 10;
        }
        else if (totalAmount > 1000)
        {
            discountPercent += 5;
        }

        if (discountPercent > 25)
        {
            discountPercent = 25;
        }

        decimal discountAmount = totalAmount * (discountPercent / 100);
        decimal finalAmount = totalAmount - discountAmount;

        order.DiscountPercent = discountPercent;
        order.DiscountAmount = discountAmount;
        order.TotalAmount = finalAmount;
        order.Status = "Processed";
    }
}
