namespace ConsoleApp.Attributes;

/// <summary>
/// Özel tasarlanmýþ Custom Attribute - Geliþtirici ve versiyon bilgisi için
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class DeveloperInfoAttribute : Attribute
{
    public string Author { get; }
    public string Version { get; }
    public string Description { get; set; } = string.Empty;
    public string LastModified { get; set; } = string.Empty;

    public DeveloperInfoAttribute(string author, string version)
    {
        Author = author;
        Version = version;
    }
}
