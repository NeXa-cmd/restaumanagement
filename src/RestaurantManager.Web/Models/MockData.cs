namespace RestaurantManager.Web.Models;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? EstimatedReadyTime { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public enum OrderStatus
{
    Preparing,
    Ready,
    Completed,
    Cancelled
}

public class CartItem
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public static class MockDataStore
{
    public static List<MenuItem> MenuItems { get; } = new()
    {
        // Appetizers
        new MenuItem { Id = 1, Name = "Spring Rolls", Description = "Crispy vegetable spring rolls", Price = 90m, Category = "Appetizers", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1529589510304-b7e994a92f60?w=800" },
        new MenuItem { Id = 2, Name = "Garlic Bread", Description = "Toasted bread with garlic butter", Price = 60m, Category = "Appetizers", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1573140401552-3fab0b24306f?w=800" },
        new MenuItem { Id = 3, Name = "Soup of the Day", Description = "Chef's special soup", Price = 70m, Category = "Appetizers", IsAvailable = false, ImageUrl = "https://images.unsplash.com/photo-1547592166-23ac45744acd?w=800" },
        new MenuItem { Id = 4, Name = "Caesar Salad", Description = "Fresh romaine with caesar dressing", Price = 100m, Category = "Appetizers", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1546793665-c74683f339c1?w=800" },
        
        // Main Course
        new MenuItem { Id = 5, Name = "Grilled Salmon", Description = "Atlantic salmon with herbs", Price = 250m, Category = "Main Course", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=800" },
        new MenuItem { Id = 6, Name = "Beef Steak", Description = "Prime ribeye steak", Price = 330m, Category = "Main Course", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1600891964092-4316c288032e?w=800" },
        new MenuItem { Id = 7, Name = "Chicken Parmesan", Description = "Breaded chicken with marinara", Price = 190m, Category = "Main Course", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1632778149955-e80f8ceca2e8?w=800" },
        new MenuItem { Id = 8, Name = "Vegetable Pasta", Description = "Penne with seasonal vegetables", Price = 160m, Category = "Main Course", IsAvailable = false, ImageUrl = "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=800" },
        new MenuItem { Id = 9, Name = "Lamb Chops", Description = "Herb-crusted lamb chops", Price = 300m, Category = "Main Course", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?w=800" },
        
        // Desserts
        new MenuItem { Id = 10, Name = "Chocolate Cake", Description = "Rich chocolate layer cake", Price = 90m, Category = "Desserts", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1624353365286-3f8d62daad51?w=800" },
        new MenuItem { Id = 11, Name = "Cheesecake", Description = "New York style cheesecake", Price = 100m, Category = "Desserts", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1533134486753-c833f0ed4866?w=800" },
        new MenuItem { Id = 12, Name = "Ice Cream Sundae", Description = "Vanilla ice cream with toppings", Price = 80m, Category = "Desserts", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1563805042-7684c019e1cb?w=800" },
        new MenuItem { Id = 13, Name = "Tiramisu", Description = "Classic Italian dessert", Price = 110m, Category = "Desserts", IsAvailable = false, ImageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=800" },
        
        // Drinks
        new MenuItem { Id = 14, Name = "Fresh Lemonade", Description = "Homemade lemonade", Price = 50m, Category = "Drinks", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1523677011781-c91d1bbe2f9d?w=800" },
        new MenuItem { Id = 15, Name = "Iced Tea", Description = "Refreshing iced tea", Price = 40m, Category = "Drinks", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=800" },
        new MenuItem { Id = 16, Name = "Coffee", Description = "Freshly brewed coffee", Price = 35m, Category = "Drinks", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1510591509098-f4fdc6d0ff04?w=800" },
        new MenuItem { Id = 17, Name = "Smoothie", Description = "Mixed berry smoothie", Price = 70m, Category = "Drinks", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1505252585461-04db1eb84625?w=800" },
        new MenuItem { Id = 18, Name = "Wine (Glass)", Description = "House red or white wine", Price = 90m, Category = "Drinks", IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?w=800" }
    };

    public static List<string> Categories { get; } = new() { "Appetizers", "Main Course", "Desserts", "Drinks" };

    public static List<Order> Orders { get; } = new()
    {
        new Order
        {
            Id = 1,
            OrderNumber = "ORD-001",
            Items = new List<OrderItem>
            {
                new OrderItem { Id = 1, MenuItemName = "Beef Steak", Quantity = 1, Price = 330m },
                new OrderItem { Id = 2, MenuItemName = "Caesar Salad", Quantity = 1, Price = 100m }
            },
            TotalPrice = 430m,
            Status = OrderStatus.Preparing,
            OrderDate = DateTime.Now.AddMinutes(-15),
            EstimatedReadyTime = DateTime.Now.AddMinutes(20)
        },
        new Order
        {
            Id = 2,
            OrderNumber = "ORD-002",
            Items = new List<OrderItem>
            {
                new OrderItem { Id = 3, MenuItemName = "Grilled Salmon", Quantity = 2, Price = 500m },
                new OrderItem { Id = 4, MenuItemName = "Wine (Glass)", Quantity = 2, Price = 180m }
            },
            TotalPrice = 680m,
            Status = OrderStatus.Preparing,
            OrderDate = DateTime.Now.AddMinutes(-5),
            EstimatedReadyTime = DateTime.Now.AddMinutes(35)
        },
        new Order
        {
            Id = 3,
            OrderNumber = "ORD-003",
            Items = new List<OrderItem>
            {
                new OrderItem { Id = 5, MenuItemName = "Chicken Parmesan", Quantity = 1, Price = 190m },
                new OrderItem { Id = 6, MenuItemName = "Chocolate Cake", Quantity = 1, Price = 90m }
            },
            TotalPrice = 280m,
            Status = OrderStatus.Completed,
            OrderDate = DateTime.Now.AddDays(-1),
            EstimatedReadyTime = null
        },
        new Order
        {
            Id = 4,
            OrderNumber = "ORD-004",
            Items = new List<OrderItem>
            {
                new OrderItem { Id = 7, MenuItemName = "Spring Rolls", Quantity = 2, Price = 180m },
                new OrderItem { Id = 8, MenuItemName = "Vegetable Pasta", Quantity = 1, Price = 160m }
            },
            TotalPrice = 340m,
            Status = OrderStatus.Completed,
            OrderDate = DateTime.Now.AddDays(-3),
            EstimatedReadyTime = null
        },
        new Order
        {
            Id = 5,
            OrderNumber = "ORD-005",
            Items = new List<OrderItem>
            {
                new OrderItem { Id = 9, MenuItemName = "Lamb Chops", Quantity = 1, Price = 300m }
            },
            TotalPrice = 300m,
            Status = OrderStatus.Completed,
            OrderDate = DateTime.Now.AddDays(-7),
            EstimatedReadyTime = null
        }
    };

    public static List<CartItem> CartItems { get; set; } = new()
    {
        new CartItem { Id = 1, MenuItemId = 6, Name = "Beef Steak", Price = 330m, Quantity = 1 },
        new CartItem { Id = 2, MenuItemId = 4, Name = "Caesar Salad", Price = 100m, Quantity = 2 },
        new CartItem { Id = 3, MenuItemId = 16, Name = "Coffee", Price = 35m, Quantity = 2 }
    };
}
