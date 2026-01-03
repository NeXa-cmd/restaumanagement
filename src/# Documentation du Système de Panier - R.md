# Documentation du Système de Panier - RestaurantManager

## Table des Matières
1. [Architecture du Panier](#architecture-du-panier)
2. [Modèle de Données](#modèle-de-données)
3. [Flux de Données](#flux-de-données)
4. [Implémentation Code](#implémentation-code)
5. [Session Utilisateur](#session-utilisateur)
6. [Schéma Complet](#schéma-complet)
7. [Points Clés](#points-clés)

---

## Architecture du Panier

Le système de panier utilise une architecture Clean Architecture avec trois couches principales:

```
Domain Layer (Entités)
    ├── Cart.cs
    ├── CartItem.cs
    ├── MenuItem.cs
    └── User.cs

Infrastructure Layer (Persistance)
    └── RestaurantDbContext.cs

Web Layer (Interface)
    ├── Pages/Index.cshtml (Menu)
    └── Pages/Cart.cshtml (Panier)
```

---

## Modèle de Données

### Structure des Tables SQL

```sql
-- Table Cart (Panier)
CREATE TABLE Carts (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Table CartItem (Article du panier)
CREATE TABLE CartItems (
    Id INT PRIMARY KEY IDENTITY,
    CartId INT NOT NULL,
    MenuItemId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (CartId) REFERENCES Carts(Id) ON DELETE CASCADE,
    FOREIGN KEY (MenuItemId) REFERENCES MenuItems(Id)
);

-- Table MenuItem (Article du menu)
CREATE TABLE MenuItems (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(18,2) NOT NULL,
    Category NVARCHAR(50),
    ImageUrl NVARCHAR(500),
    IsAvailable BIT NOT NULL
);
```

### Relations Entity Framework

```csharp
// Cart.cs (Domain/Entities/Cart.cs)
public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}

// CartItem.cs (Domain/Entities/CartItem.cs)
public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    // Navigation properties
    public Cart Cart { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
}

// MenuItem.cs (Domain/Entities/MenuItem.cs)
public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
}
```

---

## Flux de Données

### Diagramme de Séquence

```
Utilisateur → Login Page
    ↓
    [Authentification]
    ↓
Session.SetInt32("UserId", user.Id)
    ↓
Utilisateur → Menu Page (Index.cshtml)
    ↓
    [Affichage des MenuItems depuis DB]
    ↓
Utilisateur clique "Add to Cart"
    ↓
POST /Index?handler=AddToCart&menuItemId=X
    ↓
OnPostAddToCartAsync(menuItemId)
    ↓
    [1] Récupération UserId depuis Session
    ↓
    [2] Recherche Cart existant pour UserId
    ↓
    [3] Si pas de Cart → Créer nouveau Cart
    ↓
    [4] Vérifier si MenuItem existe dans CartItems
    ↓
    [5] Si existe → Quantity++
        Si non → Créer nouveau CartItem
    ↓
SaveChangesAsync() → Azure SQL Database
    ↓
Utilisateur → Cart Page (/Cart)
    ↓
    [Chargement Cart avec Include(CartItems).ThenInclude(MenuItem)]
    ↓
Affichage des articles avec calculs (Subtotal, Tax, Total)
```

---

## Implémentation Code

### 1. Ajouter au Panier (Index.cshtml.cs)

```csharp
// filepath: src/RestaurantManager.Web/Pages/Index.cshtml.cs

public class IndexModel : PageModel
{
    private readonly RestaurantDbContext _context;

    public IndexModel(RestaurantDbContext context)
    {
        _context = context;
    }

    public List<MenuItem> MenuItems { get; set; } = new();
    public List<string> Categories { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public List<string> SelectedCategories { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Charger toutes les catégories
        Categories = await _context.MenuItems
            .Select(m => m.Category)
            .Distinct()
            .ToListAsync();

        // Charger les items filtrés
        var query = _context.MenuItems.AsQueryable();

        if (SelectedCategories.Any())
        {
            query = query.Where(m => SelectedCategories.Contains(m.Category));
        }

        MenuItems = await query.ToListAsync();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int menuItemId)
    {
        // 1. Vérifier l'authentification
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // 2. Trouver ou créer le panier de l'utilisateur
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(); // Nécessaire pour obtenir cart.Id
        }

        // 3. Vérifier si l'article existe déjà dans le panier
        var existingItem = cart.CartItems?
            .FirstOrDefault(ci => ci.MenuItemId == menuItemId);

        if (existingItem != null)
        {
            // Article existe → Augmenter la quantité
            existingItem.Quantity++;
        }
        else
        {
            // Nouvel article → Créer CartItem
            var menuItem = await _context.MenuItems.FindAsync(menuItemId);
            if (menuItem == null || !menuItem.IsAvailable)
            {
                return RedirectToPage(); // Article non disponible
            }

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                MenuItemId = menuItemId,
                Quantity = 1,
                Price = menuItem.Price // Prix au moment de l'ajout
            };
            _context.CartItems.Add(cartItem);
        }

        // 4. Sauvegarder les changements dans la base de données
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}
```

### 2. Afficher le Panier (Cart.cshtml.cs)

```csharp
// filepath: src/RestaurantManager.Web/Pages/Cart.cshtml.cs

public class CartModel : PageModel
{
    private readonly RestaurantDbContext _context;

    public CartModel(RestaurantDbContext context)
    {
        _context = context;
    }

    public Cart Cart { get; set; } = new Cart();

    public async Task OnGetAsync()
    {
        // 1. Récupérer l'utilisateur depuis la session
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            Cart = new Cart { CartItems = new List<CartItem>() };
            return;
        }

        // 2. Charger le panier avec les articles et leurs détails
        Cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.MenuItem) // Charger les détails MenuItem
            .FirstOrDefaultAsync(c => c.UserId == userId.Value)
            ?? new Cart { CartItems = new List<CartItem>() };
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartItemId, int quantity)
    {
        if (quantity <= 0)
        {
            return await OnPostRemoveItemAsync(cartItemId);
        }

        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(int cartItemId)
    {
        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    // Propriétés calculées pour l'affichage
    public decimal Subtotal => Cart.CartItems?.Sum(ci => ci.Price * ci.Quantity) ?? 0;
    public decimal Tax => Subtotal * 0.10m; // 10% TVA
    public decimal Total => Subtotal + Tax;
}
```

### 3. Vue Menu (Index.cshtml)

```html
<!-- filepath: src/RestaurantManager.Web/Pages/Index.cshtml -->

@page
@model IndexModel
@{
    ViewData["Title"] = "Menu";
}

<div class="menu-page">
    <div class="menu-header">
        <h1>Our Menu</h1>
        <p>Discover our delicious selection</p>
    </div>

    <div class="menu-content">
        <!-- Sidebar avec filtres -->
        <aside class="menu-sidebar">
            <h3>Categories</h3>
            <form method="get">
                @foreach (var category in Model.Categories)
                {
                    <label class="category-filter">
                        <input type="checkbox" 
                               name="SelectedCategories" 
                               value="@category"
                               @(Model.SelectedCategories.Contains(category) ? "checked" : "")>
                        @category
                    </label>
                }
                <button type="submit" class="btn-filter">Apply Filters</button>
            </form>
        </aside>

        <!-- Grille des items -->
        <div class="menu-grid">
            @foreach (var item in Model.MenuItems)
            {
                <div class="menu-card @(!item.IsAvailable ? "unavailable" : "")">
                    @if (!string.IsNullOrEmpty(item.ImageUrl))
                    {
                        <img src="@item.ImageUrl" alt="@item.Name">
                    }
                    <div class="menu-card-content">
                        <h3>@item.Name</h3>
                        <p>@item.Description</p>
                        <div class="menu-card-footer">
                            <span class="price">$@item.Price.ToString("F2")</span>
                            @if (item.IsAvailable)
                            {
                                <form method="post" asp-page-handler="AddToCart">
                                    <input type="hidden" name="menuItemId" value="@item.Id" />
                                    <button type="submit" class="btn-add">Add to Cart</button>
                                </form>
                            }
                            else
                            {
                                <span class="unavailable-badge">Unavailable</span>
                            }
                        </div>
                    </div>
                </div>
            }
        </div>
    </div>
</div>
```

### 4. Vue Panier (Cart.cshtml)

```html
<!-- filepath: src/RestaurantManager.Web/Pages/Cart.cshtml -->

@page
@model CartModel
@{
    ViewData["Title"] = "Shopping Cart";
}

<div class="cart-page">
    <h1>Your Cart</h1>

    @if (!Model.Cart.CartItems.Any())
    {
        <div class="empty-cart">
            <p>Your cart is empty</p>
            <a href="/" class="btn-primary">Browse Menu</a>
        </div>
    }
    else
    {
        <div class="cart-items">
            @foreach (var item in Model.Cart.CartItems)
            {
                <div class="cart-item">
                    <div class="item-details">
                        <h3>@item.MenuItem.Name</h3>
                        <p class="item-price">$@item.Price.ToString("F2")</p>
                    </div>
                    
                    <div class="item-quantity">
                        <form method="post" asp-page-handler="UpdateQuantity">
                            <input type="hidden" name="cartItemId" value="@item.Id" />
                            <button type="submit" name="quantity" value="@(item.Quantity - 1)">-</button>
                            <span>@item.Quantity</span>
                            <button type="submit" name="quantity" value="@(item.Quantity + 1)">+</button>
                        </form>
                    </div>

                    <div class="item-total">
                        <span>$@((item.Price * item.Quantity).ToString("F2"))</span>
                    </div>

                    <form method="post" asp-page-handler="RemoveItem">
                        <input type="hidden" name="cartItemId" value="@item.Id" />
                        <button type="submit" class="btn-remove">Remove</button>
                    </form>
                </div>
            }
        </div>

        <div class="cart-summary">
            <div class="summary-row">
                <span>Subtotal:</span>
                <span>$@Model.Subtotal.ToString("F2")</span>
            </div>
            <div class="summary-row">
                <span>Tax (10%):</span>
                <span>$@Model.Tax.ToString("F2")</span>
            </div>
            <div class="summary-row total">
                <span>Total:</span>
                <span>$@Model.Total.ToString("F2")</span>
            </div>
            <button class="btn-checkout">Proceed to Checkout</button>
        </div>
    }
</div>
```

---

## Session Utilisateur

### Configuration (Program.cs)

```csharp
// filepath: src/RestaurantManager.Web/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services de session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Durée de session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Activer le middleware de session
app.UseSession();

app.Run();
```

### Utilisation dans Login

```csharp
// filepath: src/RestaurantManager.Web/Pages/Account/Login.cshtml.cs

public async Task<IActionResult> OnPostAsync()
{
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == Input.Email);

    if (user != null && user.PasswordHash == Input.Password)
    {
        // Stocker l'utilisateur dans la session
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserEmail", user.Email);
        
        return RedirectToPage("/Index");
    }

    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    return Page();
}
```

### Récupération dans d'autres pages

```csharp
// Dans n'importe quelle PageModel
var userId = HttpContext.Session.GetInt32("UserId");
var userEmail = HttpContext.Session.GetString("UserEmail");

if (userId == null)
{
    return RedirectToPage("/Account/Login");
}
```

---

## Schéma Complet

### Flux Détaillé d'Ajout au Panier

```
┌─────────────────────────────────────────────────────────────┐
│ 1. UTILISATEUR CONNECTÉ                                     │
│    Session: { UserId: 1, Email: "member@gmail.com" }       │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 2. PAGE MENU (Index.cshtml)                                 │
│    - Affichage de tous les MenuItems                        │
│    - Filtres par catégorie                                  │
│    - Bouton "Add to Cart" pour chaque item                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 3. CLIC SUR "Add to Cart" (MenuItem.Id = 5)                │
│    POST /Index?handler=AddToCart&menuItemId=5               │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 4. OnPostAddToCartAsync(menuItemId: 5)                      │
│                                                              │
│    a) Récupération UserId depuis Session                    │
│       → userId = 1                                           │
│                                                              │
│    b) Recherche Cart pour UserId = 1                        │
│       SQL: SELECT * FROM Carts                              │
│            WHERE UserId = 1                                  │
│       → Cart trouvé ? Oui/Non                               │
│                                                              │
│    c) Si Non: Créer nouveau Cart                            │
│       SQL: INSERT INTO Carts (UserId, CreatedAt)            │
│            VALUES (1, '2025-12-25 10:30:00')                │
│       → cart.Id = 42                                         │
│                                                              │
│    d) Vérifier si MenuItem 5 existe dans CartItems          │
│       → existingItem = CartItems.Find(mi => mi.Id == 5)     │
│                                                              │
│    e) Si existe:                                             │
│          existingItem.Quantity++                            │
│       Sinon:                                                 │
│          Créer nouveau CartItem                             │
│          SQL: INSERT INTO CartItems                         │
│               (CartId, MenuItemId, Quantity, Price)         │
│               VALUES (42, 5, 1, 12.99)                      │
│                                                              │
│    f) SaveChangesAsync()                                    │
│       → Commit vers Azure SQL Database                      │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 5. REDIRECTION vers Page Menu                               │
│    RedirectToPage() → /Index                                │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 6. UTILISATEUR NAVIGUE vers /Cart                           │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 7. OnGetAsync() dans Cart.cshtml.cs                         │
│                                                              │
│    a) Récupération UserId depuis Session                    │
│       → userId = 1                                           │
│                                                              │
│    b) Chargement Cart avec Items                            │
│       SQL: SELECT c.*, ci.*, mi.*                           │
│            FROM Carts c                                      │
│            JOIN CartItems ci ON c.Id = ci.CartId            │
│            JOIN MenuItems mi ON ci.MenuItemId = mi.Id       │
│            WHERE c.UserId = 1                                │
│                                                              │
│    c) Calculs:                                               │
│       Subtotal = Σ(ci.Price × ci.Quantity)                  │
│       Tax = Subtotal × 0.10                                 │
│       Total = Subtotal + Tax                                │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 8. AFFICHAGE Cart.cshtml                                    │
│    - Liste des CartItems avec détails MenuItem              │
│    - Contrôles de quantité (+/-)                            │
│    - Bouton "Remove"                                         │
│    - Résumé: Subtotal, Tax, Total                           │
└─────────────────────────────────────────────────────────────┘
```

### État de la Base de Données Après Ajout

```sql
-- Table Users
Id | Email               | PasswordHash | Name
1  | member@gmail.com    | member123    | Member User

-- Table Carts
Id | UserId | CreatedAt
42 | 1      | 2025-12-25 10:30:00

-- Table CartItems
Id | CartId | MenuItemId | Quantity | Price
1  | 42     | 5          | 1        | 12.99
2  | 42     | 8          | 2        | 8.50

-- Table MenuItems
Id | Name              | Price | Category    | IsAvailable
5  | Caesar Salad      | 12.99 | Appetizers  | 1
8  | French Fries      | 8.50  | Sides       | 1
```

---

## Points Clés

### 1. **Authentification**
- Utilise ASP.NET Core Session
- Stocke `UserId` et `UserEmail`
- Timeout: 30 minutes
- Cookie HttpOnly pour sécurité

### 2. **Persistance**
- Entity Framework Core
- Azure SQL Database
- Relations: `Cart` → `CartItems` → `MenuItems`
- Cascade Delete sur CartItems

### 3. **Logique Métier**

| Scénario | Action |
|----------|--------|
| Premier ajout au panier | Créer Cart + CartItem |
| Article déjà dans panier | Incrémenter Quantity |
| Quantité = 0 | Supprimer CartItem |
| MenuItem indisponible | Bloquer l'ajout |

### 4. **Performance**
- `Include()` pour charger relations (évite N+1)
- `ThenInclude()` pour sous-relations
- `AsNoTracking()` possible pour lecture seule
- Index sur `UserId` et `CartId`

### 5. **Sécurité**
- Vérification de session avant chaque action
- Validation des IDs (FindAsync)
- Prix stocké au moment de l'ajout (évite changements)
- CSRF protection (Razor Pages auto)

### 6. **Extensibilité Future**

Fonctionnalités à ajouter:

```csharp
// Validation de stock
if (menuItem.Stock < quantity)
{
    return Error("Insufficient stock");
}

// Limites de quantité
if (quantity > 10)
{
    return Error("Maximum 10 items per order");
}

// Code promo
decimal discount = ApplyPromoCode(Cart, promoCode);

// Conversion Cart → Order
var order = new Order
{
    UserId = userId,
    OrderItems = Cart.CartItems.Select(ci => new OrderItem
    {
        MenuItemId = ci.MenuItemId,
        Quantity = ci.Quantity,
        Price = ci.Price
    }).ToList(),
    Status = OrderStatus.Pending
};
_context.Orders.Add(order);
_context.Carts.Remove(Cart); // Vider le panier
await _context.SaveChangesAsync();
```

---

## Résumé Technique

| Composant | Technologie | Rôle |
|-----------|-------------|------|
| **Frontend** | Razor Pages | Interface utilisateur |
| **Backend** | ASP.NET Core | Logique métier |
| **ORM** | Entity Framework Core | Mapping objet-relationnel |
| **Database** | Azure SQL Database | Persistance données |
| **Auth** | ASP.NET Session | Gestion utilisateur |
| **Architecture** | Clean Architecture | Séparation des couches |

### Stack Technique Complète

```
┌─────────────────────────────────────┐
│  Razor Pages (.cshtml)              │  ← Interface utilisateur
├─────────────────────────────────────┤
│  PageModel (.cshtml.cs)             │  ← Logique de présentation
├─────────────────────────────────────┤
│  DbContext (RestaurantDbContext)    │  ← Accès aux données
├─────────────────────────────────────┤
│  Entity Framework Core              │  ← ORM
├─────────────────────────────────────┤
│  Azure SQL Database                 │  ← Stockage
└─────────────────────────────────────┘
```

---

## Conclusion

Le système de panier fonctionne avec:
1. **Session** pour identifier l'utilisateur
2. **Entity Framework** pour gérer les données
3. **Azure SQL** pour persister les paniers
4. **Razor Pages** pour l'interface

Chaque action (ajout, modification, suppression) passe par:
- Vérification de session
- Manipulation des entités EF Core
- Sauvegarde dans la base de données
- Rafraîchissement de la vue

**Documentation créée le:** 25 décembre 2025  
**Version de l'application:** 1.0  
**Base de données:** olkad.database.windows.net