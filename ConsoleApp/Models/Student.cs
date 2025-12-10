namespace ConsoleApp.Models;

/// <summary>
/// Öðrenciyi temsil eden struct (deðer tipi - value type)
/// Struct'lar stack'te tutulur ve kopyalandýðýnda deðer olarak kopyalanýr
/// </summary>
public struct Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Gpa { get; set; } // Not Ortalamasý (Grade Point Average)

    public Student(int id, string name, double gpa)
    {
        Id = id;
        Name = name;
        Gpa = gpa;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Ad: {Name}, Not Ortalamasý: {Gpa:F2}";
    }
}
