using System;

public class Student
{
    // Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public double Grade { get; set; }

    // Constructor
    public Student(int id, string name, int age, double grade)
    {
        Id = id;
        Name = name;
        Age = age;
        Grade = grade;
    }

    // Method to display student info
    public void DisplayInfo()
    {
        Console.WriteLine($"IDdsadasdsadas: {Id}");
        Console.WriteLine($"Nameasdasdsadasdasd: {Name}");
        Console.WriteLine($"Ageasdsadasdasd: {Age}");
        Console.WriteLine($"Gradeadsadasdsadasdsa: {Grade}");
    }

    // Method to check if student passed
    public bool IsPassed()
    {
        return Grade >= 50;
    }
}