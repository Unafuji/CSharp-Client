using System;

public class Customer
{
    // Properties
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    // Constructor
    public Customer(int customerId, string name, string email, string phone)
    {
        CustomerId = customerId;
        Name = name;
        Email = email;
        Phone = phone;
    }

    // Display customer information
    public void DisplayInfo()
    {
        Console.WriteLine($"Customer ID: {CustomerId}");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Email: {Email}");
        Console.WriteLine($"Phone: {Phone}");
    }

    // Method to validate email format (simple check)
    public bool IsValidEmail()
    {
        return Email.Contains("@") && Email.Contains(".");
    }
}