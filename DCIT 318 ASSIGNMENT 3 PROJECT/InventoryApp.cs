using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable record for inventory items
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item) => _log.Add(item);

    public bool Remove(int id)
    {
        var item = _log.Find(x => x.Id == id);
        if (item != null)
        {
            _log.Remove(item);
            return true;
        }
        return false;
    }

    public bool Update(int id, T newItem)
    {
        int index = _log.FindIndex(x => x.Id == id);
        if (index >= 0)
        {
            _log[index] = newItem;
            return true;
        }
        return false;
    }

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                writer.Write(json);
            }
            Console.WriteLine("✅ Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving data: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("⚠ No data file found.");
                return;
            }

            using (StreamReader reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                if (items != null)
                {
                    _log = items;
                    Console.WriteLine("✅ Data loaded successfully.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading data: {ex.Message}");
        }
    }
}

// Integration Layer
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void AddItem(int id, string name, int quantity)
    {
        _logger.Add(new InventoryItem(id, name, quantity, DateTime.Now));
    }

    public void UpdateItem(int id, string newName, int newQuantity)
    {
        var existing = _logger.GetAll().Find(x => x.Id == id);
        if (existing != null)
        {
            var updated = existing with { Name = newName, Quantity = newQuantity };
            if (_logger.Update(id, updated))
                Console.WriteLine("✅ Item updated successfully.");
        }
        else
        {
            Console.WriteLine("❌ Item not found.");
        }
    }

    public void DeleteItem(int id)
    {
        if (_logger.Remove(id))
            Console.WriteLine("✅ Item deleted successfully.");
        else
            Console.WriteLine("❌ Item not found.");
    }

    public void ViewItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("⚠ No inventory items found.");
            return;
        }
        Console.WriteLine("\n--- INVENTORY LIST ---");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();
}

// Main Program
class Program
{
    static void Main()
    {
        string filePath = "inventory_data.json";
        InventoryApp app = new InventoryApp(filePath);

        // Load existing data if available
        app.LoadData();

        while (true)
        {
            Console.WriteLine("\n--- INVENTORY MENU ---");
            Console.WriteLine("1. View Items");
            Console.WriteLine("2. Add Item");
            Console.WriteLine("3. Update Item");
            Console.WriteLine("4. Delete Item");
            Console.WriteLine("5. Save Data");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1":
                    app.ViewItems();
                    break;
                case "2":
                    Console.Write("Enter ID: ");
                    int id = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter Name: ");
                    string name = Console.ReadLine() ?? "";
                    Console.Write("Enter Quantity: ");
                    int qty = int.Parse(Console.ReadLine() ?? "0");
                    app.AddItem(id, name, qty);
                    break;
                case "3":
                    Console.Write("Enter ID to Update: ");
                    int uid = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter New Name: ");
                    string newName = Console.ReadLine() ?? "";
                    Console.Write("Enter New Quantity: ");
                    int newQty = int.Parse(Console.ReadLine() ?? "0");
                    app.UpdateItem(uid, newName, newQty);
                    break;
                case "4":
                    Console.Write("Enter ID to Delete: ");
                    int did = int.Parse(Console.ReadLine() ?? "0");
                    app.DeleteItem(did);
                    break;
                case "5":
                    app.SaveData();
                    break;
                case "6":
                    app.SaveData();
                    Console.WriteLine("💾 Data saved. Goodbye!");
                    return;
                default:
                    Console.WriteLine("❌ Invalid choice, try again.");
                    break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable record for inventory items
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item) => _log.Add(item);

    public bool Remove(int id)
    {
        var item = _log.Find(x => x.Id == id);
        if (item != null)
        {
            _log.Remove(item);
            return true;
        }
        return false;
    }

    public bool Update(int id, T newItem)
    {
        int index = _log.FindIndex(x => x.Id == id);
        if (index >= 0)
        {
            _log[index] = newItem;
            return true;
        }
        return false;
    }

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                writer.Write(json);
            }
            Console.WriteLine("✅ Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving data: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("⚠ No data file found.");
                return;
            }

            using (StreamReader reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                if (items != null)
                {
                    _log = items;
                    Console.WriteLine("✅ Data loaded successfully.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading data: {ex.Message}");
        }
    }
}

// Integration Layer
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void AddItem(int id, string name, int quantity)
    {
        _logger.Add(new InventoryItem(id, name, quantity, DateTime.Now));
    }

    public void UpdateItem(int id, string newName, int newQuantity)
    {
        var existing = _logger.GetAll().Find(x => x.Id == id);
        if (existing != null)
        {
            var updated = existing with { Name = newName, Quantity = newQuantity };
            if (_logger.Update(id, updated))
                Console.WriteLine("✅ Item updated successfully.");
        }
        else
        {
            Console.WriteLine("❌ Item not found.");
        }
    }

    public void DeleteItem(int id)
    {
        if (_logger.Remove(id))
            Console.WriteLine("✅ Item deleted successfully.");
        else
            Console.WriteLine("❌ Item not found.");
    }

    public void ViewItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("⚠ No inventory items found.");
            return;
        }
        Console.WriteLine("\n--- INVENTORY LIST ---");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();
}

// Main Program
class Program
{
    static void Main()
    {
        string filePath = "inventory_data.json";
        InventoryApp app = new InventoryApp(filePath);

        // Load existing data if available
        app.LoadData();

        while (true)
        {
            Console.WriteLine("\n--- INVENTORY MENU ---");
            Console.WriteLine("1. View Items");
            Console.WriteLine("2. Add Item");
            Console.WriteLine("3. Update Item");
            Console.WriteLine("4. Delete Item");
            Console.WriteLine("5. Save Data");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1":
                    app.ViewItems();
                    break;
                case "2":
                    Console.Write("Enter ID: ");
                    int id = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter Name: ");
                    string name = Console.ReadLine() ?? "";
                    Console.Write("Enter Quantity: ");
                    int qty = int.Parse(Console.ReadLine() ?? "0");
                    app.AddItem(id, name, qty);
                    break;
                case "3":
                    Console.Write("Enter ID to Update: ");
                    int uid = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter New Name: ");
                    string newName = Console.ReadLine() ?? "";
                    Console.Write("Enter New Quantity: ");
                    int newQty = int.Parse(Console.ReadLine() ?? "0");
                    app.UpdateItem(uid, newName, newQty);
                    break;
                case "4":
                    Console.Write("Enter ID to Delete: ");
                    int did = int.Parse(Console.ReadLine() ?? "0");
                    app.DeleteItem(did);
                    break;
                case "5":
                    app.SaveData();
                    break;
                case "6":
                    app.SaveData();
                    Console.WriteLine("💾 Data saved. Goodbye!");
                    return;
                default:
                    Console.WriteLine("❌ Invalid choice, try again.");
                    break;
            }
        }
    }
}
