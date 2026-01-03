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
        new MenuItem { Id = 1, Name = "Spring Rolls", Description = "Crispy vegetable spring rolls", Price = 8.99m, Category = "Appetizers", IsAvailable = true, ImageUrl = "🥟" },
        new MenuItem { Id = 2, Name = "Garlic Bread", Description = "Toasted bread with garlic butter", Price = 5.99m, Category = "Appetizers", IsAvailable = true, ImageUrl = "🥖" },
        new MenuItem { Id = 3, Name = "Soup of the Day", Description = "Chef's special soup", Price = 6.99m, Category = "Appetizers", IsAvailable = false, ImageUrl = "🍜" },
        new MenuItem { Id = 4, Name = "Caesar Salad", Description = "Fresh romaine with caesar dressing", Price = 9.99m, Category = "Appetizers", IsAvailable = true, ImageUrl = "🥗" },
        
        // Main Course
        new MenuItem { Id = 5, Name = "Grilled Salmon", Description = "Atlantic salmon with herbs", Price = 24.99m, Category = "Main Course", IsAvailable = true, ImageUrl = "🐟" },
        new MenuItem { Id = 6, Name = "Beef Steak", Description = "Prime ribeye steak", Price = 32.99m, Category = "Main Course", IsAvailable = true, ImageUrl = "🥩" },
        new MenuItem { Id = 7, Name = "Chicken Parmesan", Description = "Breaded chicken with marinara", Price = 18.99m, Category = "Main Course", IsAvailable = true, ImageUrl = "🍗" },
        new MenuItem { Id = 8, Name = "Vegetable Pasta", Description = "Penne with seasonal vegetables", Price = 15.99m, Category = "Main Course", IsAvailable = false, ImageUrl = "🍝" },
        new MenuItem { Id = 9, Name = "Lamb Chops", Description = "Herb-crusted lamb chops", Price = 29.99m, Category = "Main Course", IsAvailable = true, ImageUrl = "🍖" },
        
        // Desserts
        new MenuItem { Id = 10, Name = "Chocolate Cake", Description = "Rich chocolate layer cake", Price = 8.99m, Category = "Desserts", IsAvailable = true, ImageUrl = "🍫" },
        new MenuItem { Id = 11, Name = "Cheesecake", Description = "New York style cheesecake", Price = 9.99m, Category = "Desserts", IsAvailable = true, ImageUrl = "🍰" },
        new MenuItem { Id = 12, Name = "Ice Cream Sundae", Description = "Vanilla ice cream with toppings", Price = 7.99m, Category = "Desserts", IsAvailable = true, ImageUrl = "🍨" },
        new MenuItem { Id = 13, Name = "Tiramisu", Description = "Classic Italian dessert", Price = 10.99m, Category = "Desserts", IsAvailable = false, ImageUrl = "☕" },
        
        // Drinks
        new MenuItem { Id = 14, Name = "Fresh Lemonade", Description = "Homemade lemonade", Price = 4.99m, Category = "Drinks", IsAvailable = true, ImageUrl = "🍋" },
        new MenuItem { Id = 15, Name = "Iced Tea", Description = "Refreshing iced tea", Price = 3.99m, Category = "Drinks", IsAvailable = true, ImageUrl = "🧊" },
        new MenuItem { Id = 16, Name = "Coffee", Description = "Freshly brewed coffee", Price = 3.49m, Category = "Drinks", IsAvailable = true, ImageUrl = "☕" },
        new MenuItem { Id = 17, Name = "Smoothie", Description = "Mixed berry smoothie", Price = 6.99m, Category = "Drinks", IsAvailable = true, ImageUrl = "🥤" },
        new MenuItem { Id = 18, Name = "Wine (Glass)", Description = "House red or white wine", Price = 8.99m, Category = "Drinks", IsAvailable = true, ImageUrl = "🍷" }
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
                new OrderItem { Id = 1, MenuItemName = "Beef Steak", Quantity = 1, Price = 32.99m },
                new OrderItem { Id = 2, MenuItemName = "Caesar Salad", Quantity = 1, Price = 9.99m }
            },
            TotalPrice = 42.98m,
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
                new OrderItem { Id = 3, MenuItemName = "Grilled Salmon", Quantity = 2, Price = 49.98m },
                new OrderItem { Id = 4, MenuItemName = "Wine (Glass)", Quantity = 2, Price = 17.98m }
            },
            TotalPrice = 67.96m,
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
                new OrderItem { Id = 5, MenuItemName = "Chicken Parmesan", Quantity = 1, Price = 18.99m },
                new OrderItem { Id = 6, MenuItemName = "Chocolate Cake", Quantity = 1, Price = 8.99m }
            },
            TotalPrice = 27.98m,
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
                new OrderItem { Id = 7, MenuItemName = "Spring Rolls", Quantity = 2, Price = 17.98m },
                new OrderItem { Id = 8, MenuItemName = "Vegetable Pasta", Quantity = 1, Price = 15.99m }
            },
            TotalPrice = 33.97m,
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
                new OrderItem { Id = 9, MenuItemName = "Lamb Chops", Quantity = 1, Price = 29.99m }
            },
            TotalPrice = 29.99m,
            Status = OrderStatus.Completed,
            OrderDate = DateTime.Now.AddDays(-7),
            EstimatedReadyTime = null
        }
    };

    public static List<CartItem> CartItems { get; set; } = new()
    {
        new CartItem { Id = 1, MenuItemId = 6, Name = "Beef Steak", Price = 32.99m, Quantity = 1 },
        new CartItem { Id = 2, MenuItemId = 4, Name = "Caesar Salad", Price = 9.99m, Quantity = 2 },
        new CartItem { Id = 3, MenuItemId = 16, Name = "Coffee", Price = 3.49m, Quantity = 2 }
    };
}
