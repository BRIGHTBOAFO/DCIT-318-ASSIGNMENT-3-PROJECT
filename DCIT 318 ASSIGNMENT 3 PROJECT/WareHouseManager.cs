using System;
using System.Collections.Generic;

// ---------------------- Marker Interface ----------------------
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// ---------------------- Product Classes ----------------------
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"[Electronic] ID: {Id}, Name: {Name}, Qty: {Quantity}, Brand: {Brand}, Warranty: {WarrantyMonths} months";
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"[Grocery] ID: {Id}, Name: {Name}, Qty: {Quantity}, Expiry: {ExpiryDate.ToShortDateString()}";
    }
}

// ---------------------- Custom Exceptions ----------------------
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// ---------------------- Generic Inventory Repository ----------------------
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        _items.Remove(id);
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        _items[id].Quantity = newQuantity;
    }
}

// ---------------------- Warehouse Manager ----------------------
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));

        _groceries.AddItem(new GroceryItem(1, "Rice", 50, DateTime.Now.AddMonths(12)));
        _groceries.AddItem(new GroceryItem(2, "Milk", 20, DateTime.Now.AddDays(10)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine(item);
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock increased for ID {id}. New quantity: {item.Quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    public InventoryRepository<ElectronicItem> GetElectronicsRepo() => _electronics;
    public InventoryRepository<GroceryItem> GetGroceriesRepo() => _groceries;
}

// ---------------------- Main Application ----------------------
public class Program
{
    public static void Main()
    {
        WareHouseManager manager = new WareHouseManager();

        // Seed data
        manager.SeedData();

        // Print all grocery items
        Console.WriteLine("GROCERY ITEMS:");
        manager.PrintAllItems(manager.GetGroceriesRepo());

        Console.WriteLine("\nELECTRONIC ITEMS:");
        manager.PrintAllItems(manager.GetElectronicsRepo());

        Console.WriteLine("\n--- Testing Exceptions ---");

        // 1. Add a duplicate item
        try
        {
            manager.GetGroceriesRepo().AddItem(new GroceryItem(1, "Beans", 10, DateTime.Now.AddMonths(6)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Duplicate test: {ex.Message}");
        }

        // 2. Remove a non-existent item
        manager.RemoveItemById(manager.GetElectronicsRepo(), 99);

        // 3. Update with invalid quantity
        try
        {
            manager.GetElectronicsRepo().UpdateQuantity(1, -5);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Invalid quantity test: {ex.Message}");
        }
    }
}
