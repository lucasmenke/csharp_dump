using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Shared;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;

    // Data Annotation is only needed when the class will be mapped to a database
    // OrderOverviewResponse is a DTO and don´t need it
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}
