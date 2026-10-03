namespace starterkit.Data.Models.Orders;

public class OrderApiResponse
{
  public List<OrderItem> Items { get; set; } = new();
  public PaginationMetadata Metadata { get; set; } = new();
}

