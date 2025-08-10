namespace RefactoringChallenge.Model.Domain;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Status { get; set; }
    public List<OrderItem> Items { get; set; }
}
