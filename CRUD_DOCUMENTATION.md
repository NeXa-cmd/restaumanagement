# Admin CRUD Operations - Restaurant Management System

## Overview
This document describes the complete CRUD (Create, Read, Update, Delete) operations implemented for the Restaurant Management System admin panel.

---

## ?? Implemented Features

### 1. **Admin Dashboard** (`/Admin`)
- Overview statistics:
  - Total Menu Items
  - Total Orders
  - Pending Orders
  - Total Categories
- Quick action cards linking to management pages

**Files:**
- `Pages/Admin/Index.cshtml`
- `Pages/Admin/Index.cshtml.cs`

---

### 2. **Menu Items Management** (`/Admin/MenuItems`)

#### Features:
- ? **List all menu items** with details (Name, Description, Category, Price, Status)
- ? **Create new menu items** with validation
- ? **Edit existing menu items**
- ? **Delete menu items** with confirmation
- ? **Toggle availability status**

#### Pages:
1. **Index** - List all menu items
   - `Pages/Admin/MenuItems/Index.cshtml[.cs]`
   
2. **Create** - Add new menu item
   - `Pages/Admin/MenuItems/Create.cshtml[.cs]`
   - Fields: Name*, Description*, Price*, Category*, ImageUrl, IsAvailable
   - Validation: Required fields, price range (0.01-10000), URL format
   
3. **Edit** - Update menu item
   - `Pages/Admin/MenuItems/Edit.cshtml[.cs]`
   - Pre-populated with existing data
   
4. **Delete** - Remove menu item
   - `Pages/Admin/MenuItems/Delete.cshtml[.cs]`
   - Displays item details with confirmation

---

### 3. **Categories Management** (`/Admin/Categories`)

#### Features:
- ? **List all categories** with display order
- ? **Create new categories**
- ? **Edit existing categories**
- ? **Delete categories** with confirmation
- ? **Set display order** for sorting
- ? **Toggle active/inactive status**

#### Pages:
1. **Index** - List all categories
   - `Pages/Admin/Categories/Index.cshtml[.cs]`
   
2. **Create** - Add new category
   - `Pages/Admin/Categories/Create.cshtml[.cs]`
   - Fields: Name*, Description, DisplayOrder*, IsActive
   - Validation: Name length (max 50), Display order (1-100)
   
3. **Edit** - Update category
   - `Pages/Admin/Categories/Edit.cshtml[.cs]`
   
4. **Delete** - Remove category
   - `Pages/Admin/Categories/Delete.cshtml[.cs]`

---

### 4. **Orders Management** (`/Admin/Orders`)

#### Features:
- ? **View all orders** with customer info
- ? **View order details** (items, quantities, prices)
- ? **Update order status** (Pending ? Preparing ? Completed)
- ? **View order history**

#### Order Statuses:
- `Pending` ? Order placed, waiting for preparation
- `Preparing` ? Being prepared in kitchen
- `Completed` ? Order finished and delivered
- `Cancelled` ? Order cancelled

#### Pages:
1. **Index** - List all orders
   - `Pages/Admin/Orders/Index.cshtml[.cs]`
   - Quick status update button
   
2. **Details** - View order details
   - `Pages/Admin/Orders/Details.cshtml[.cs]`
   - Customer information
   - Order items with quantities and prices
   - Total amount calculation

---

### 5. **Users Management** (`/Admin/Users`)

#### Features:
- ? **View all users** with registration date
- ? **View order count per user**
- ? **View account status** (Active/Inactive)

#### Pages:
1. **Index** - List all users
   - `Pages/Admin/Users/Index.cshtml[.cs]`

---

## ??? Project Structure

```
src/RestaurantManager.Web/Pages/
??? Admin/
?   ??? Index.cshtml                    # Dashboard
?   ??? Index.cshtml.cs
?   ??? MenuItems/
?   ?   ??? Index.cshtml                # List
?   ?   ??? Index.cshtml.cs
?   ?   ??? Create.cshtml               # Create
?   ?   ??? Create.cshtml.cs
?   ?   ??? Edit.cshtml                 # Update
?   ?   ??? Edit.cshtml.cs
?   ?   ??? Delete.cshtml               # Delete
?   ?   ??? Delete.cshtml.cs
?   ??? Categories/
?   ?   ??? Index.cshtml
?   ?   ??? Index.cshtml.cs
?   ?   ??? Create.cshtml
?   ?   ??? Create.cshtml.cs
?   ?   ??? Edit.cshtml
?   ?   ??? Edit.cshtml.cs
?   ?   ??? Delete.cshtml
?   ?   ??? Delete.cshtml.cs
?   ??? Orders/
?   ?   ??? Index.cshtml
?   ?   ??? Index.cshtml.cs
?   ?   ??? Details.cshtml
?   ?   ??? Details.cshtml.cs
?   ??? Users/
?       ??? Index.cshtml
?       ??? Index.cshtml.cs
```

---

## ?? Authentication

All admin pages are protected with session-based authentication:

```csharp
if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToPage("/Account/Login");
}
```

Users must be logged in to access any admin functionality.

---

## ?? UI Features

### Design System:
- Modern, clean interface with card-based layouts
- Responsive tables with hover effects
- Color-coded status badges
- Icon-based navigation
- Dark mode support
- Form validation with error messages
- Confirmation dialogs for destructive actions

### Color Coding:
- **Primary (Blue)**: Actions, links
- **Green**: Success, completed, available
- **Yellow/Orange**: Warning, pending, preparing
- **Red**: Danger, delete, cancelled, unavailable

---

## ?? Validation Rules

### Menu Items:
- **Name**: Required, max 100 characters
- **Description**: Required, max 500 characters
- **Price**: Required, between 0.01 and 10,000
- **Category**: Required
- **ImageUrl**: Optional, must be valid URL

### Categories:
- **Name**: Required, max 50 characters
- **Description**: Optional, max 200 characters
- **DisplayOrder**: Required, between 1 and 100

---

## ?? Usage

### Accessing Admin Panel:
1. Log in to the application
2. Navigate to **Admin** section in the sidebar
3. Choose the management area:
   - **Dashboard**: Overview statistics
   - **Menu Items**: Manage food items
   - **Categories**: Organize menu structure
   - **Orders**: Process customer orders
   - **Users**: View customer accounts

### Creating a Menu Item:
1. Go to `/Admin/MenuItems`
2. Click **"Add Menu Item"**
3. Fill in the form:
   - Item name
   - Description
   - Price (in MAD)
   - Select category
   - Optional: Add image URL
   - Check "Available for ordering" if ready
4. Click **"Create Menu Item"**

### Managing Orders:
1. Go to `/Admin/Orders`
2. View all orders in the table
3. Click **"View"** to see order details
4. Click **"Next Status"** to advance order status:
   - Pending ? Preparing (sets ready time)
   - Preparing ? Completed (sets completion time)

### Creating Categories:
1. Go to `/Admin/Categories`
2. Click **"Add Category"**
3. Enter name, description, and display order
4. Click **"Create Category"**

---

## ?? Order Status Workflow

```
???????????     ?????????????     ?????????????
? Pending ? --> ? Preparing ? --> ? Completed ?
???????????     ?????????????     ?????????????
                      ?
                      ?
                ?????????????
                ? Cancelled ?
                ?????????????
```

---

## ??? Technical Details

### Technologies Used:
- **ASP.NET Core 10** - Razor Pages
- **Entity Framework Core** - ORM
- **Azure SQL Database** - Data persistence
- **Session-based Authentication**
- **Client-side validation** with jQuery Validation
- **Server-side validation** with Data Annotations

### Database Operations:
- All CRUD operations use **async/await** pattern
- Entity Framework **Include()** for eager loading
- Proper **cascade delete** rules configured
- Navigation properties for relationships

### Security:
- Session validation on all admin pages
- CSRF protection (built-in with Razor Pages)
- Input validation (client + server)
- SQL injection protection (EF Core parameterized queries)

---

## ?? Database Schema

### Related Tables:
- `MenuItems` - Food items in the menu
- `Categories` - Menu categories
- `Orders` - Customer orders
- `OrderItems` - Items in each order
- `Users` - Customer accounts

### Key Relationships:
- **MenuItem** ? **Category** (Many-to-One via string)
- **Order** ? **User** (Many-to-One)
- **Order** ? **OrderItem** (One-to-Many)
- **OrderItem** ? **MenuItem** (Many-to-One)

---

## ?? Future Enhancements

Possible additions:
- [ ] Bulk operations (multi-select delete)
- [ ] Search and filtering
- [ ] Sorting columns
- [ ] Pagination for large datasets
- [ ] Export to CSV/Excel
- [ ] Image upload functionality
- [ ] Role-based access control (Admin vs Manager)
- [ ] Audit logs
- [ ] Reports and analytics
- [ ] Email notifications for orders

---

## ? Testing Checklist

### Menu Items:
- [ ] Create a new menu item
- [ ] Edit existing menu item
- [ ] Delete menu item
- [ ] Validate required fields
- [ ] Validate price range
- [ ] Toggle availability

### Categories:
- [ ] Create new category
- [ ] Edit category
- [ ] Delete category
- [ ] Change display order
- [ ] Toggle active status

### Orders:
- [ ] View all orders
- [ ] View order details
- [ ] Update order status (Pending ? Preparing)
- [ ] Update order status (Preparing ? Completed)
- [ ] Verify timestamps update correctly

### Users:
- [ ] View all users
- [ ] See order counts
- [ ] Verify registration dates

---

## ?? Support

For issues or questions about the CRUD operations, check:
1. Build errors in Visual Studio
2. Browser console for JavaScript errors
3. Application logs for server errors
4. Database connection string in `appsettings.json`

---

**Created:** December 2024  
**Version:** 1.0  
**Framework:** ASP.NET Core 10  
**Database:** Azure SQL Server
