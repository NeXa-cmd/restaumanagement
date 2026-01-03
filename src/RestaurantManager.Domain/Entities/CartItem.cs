namespace RestaurantManager.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Cart Cart { get; set; } = null!;
    public virtual MenuItem MenuItem { get; set; } = null!;
}
