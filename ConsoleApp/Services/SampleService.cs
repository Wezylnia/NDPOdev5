using ConsoleApp.Attributes;

namespace ConsoleApp.Services;

/// <summary>
/// Custom Attribute'larýn uygulandýðý örnek servis sýnýfý
/// Reflection ile bu sýnýfýn metadata'sý analiz edilecek
/// </summary>
[DeveloperInfo("Eda", "1.0.0", Description = "Örnek servis sýnýfý - CRUD iþlemleri", LastModified = "2024-01-15")]
public class SampleService
{
    [DeveloperInfo("Eda", "1.0.0", Description = "Tüm öðeleri listeler")]
    public void GetAllItems()
    {
        Console.WriteLine("GetAllItems metodu çalýþtý.");
    }

    [DeveloperInfo("Eda", "1.1.0", Description = "ID'ye göre öðe getirir", LastModified = "2024-01-10")]
    public void GetItemById(int id)
    {
        Console.WriteLine($"GetItemById metodu çalýþtý. ID: {id}");
    }

    [DeveloperInfo("Eda", "1.2.0", Description = "Yeni öðe ekler")]
    public void AddItem(string name)
    {
        Console.WriteLine($"AddItem metodu çalýþtý. Ýsim: {name}");
    }

    [DeveloperInfo("Eda", "1.0.0", Description = "Öðeyi siler")]
    public void DeleteItem(int id)
    {
        Console.WriteLine($"DeleteItem metodu çalýþtý. ID: {id}");
    }
}
