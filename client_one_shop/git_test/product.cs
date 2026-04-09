using System;

public class Product
{
    // Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }

    // Constructor
    public Product(int id, string name, double price, int stock)
    {
        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }

    // Method to display product info
    public void DisplayInfo()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Price: ${Price}");
        Console.WriteLine($"Stock: {Stock}");
    }

    // Method to check if product is in stock
    public bool IsInStock()
    {
        return Stock > 0;
    }

    // Method to reduce stock after purchase
    public void Sell(int quantity)
    {
        if (quantity <= Stock)
        {
            Stock -= quantity;
            Console.WriteLine($"Sold {quantity} item(s).");
        }
        else
        {
            Console.WriteLine("Not enough stock!");
        }
    }
}