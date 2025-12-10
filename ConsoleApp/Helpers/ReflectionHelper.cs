using System.Reflection;
using ConsoleApp.Attributes;

namespace ConsoleApp.Helpers;

/// <summary>
/// Reflection kullanarak metadata analizi yapan yardýmcý sýnýf
/// </summary>
public static class ReflectionHelper
{
    /// <summary>
    /// Verilen tip üzerindeki DeveloperInfo attribute'larýný analiz eder ve rapor oluþturur
    /// </summary>
    public static void GenerateAttributeReport(Type type)
    {
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("                    ATTRIBUTE RAPORU");
        Console.WriteLine(new string('=', 70));

        // Sinif bilgilerini al
        Console.WriteLine($"\nSinif Adi: {type.Name}");
        Console.WriteLine($"Tam Ad (FullName): {type.FullName}");
        Console.WriteLine($"Namespace: {type.Namespace}");
        Console.WriteLine($"Assembly: {type.Assembly.GetName().Name}");

        // Sinif uzerindeki DeveloperInfo attribute'unu oku
        var classAttribute = type.GetCustomAttribute<DeveloperInfoAttribute>();
        if (classAttribute != null)
        {
            Console.WriteLine("\n+---------------------------------------------------------------------+");
            Console.WriteLine("|                    SINIF ATTRIBUTE BILGILERI                       |");
            Console.WriteLine("+---------------------------------------------------------------------+");
            Console.WriteLine($"|  Yazar        : {classAttribute.Author,-50} |");
            Console.WriteLine($"|  Versiyon     : {classAttribute.Version,-50} |");
            Console.WriteLine($"|  Aciklama     : {classAttribute.Description,-50} |");
            Console.WriteLine($"|  Son Guncelleme: {classAttribute.LastModified,-49} |");
            Console.WriteLine("+---------------------------------------------------------------------+");
        }

        // Metotlari tara
        Console.WriteLine("\n" + new string('-', 70));
        Console.WriteLine("                    METOT ATTRIBUTE BILGILERI");
        Console.WriteLine(new string('-', 70));

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        
        int methodCount = 0;
        foreach (var method in methods)
        {
            var methodAttribute = method.GetCustomAttribute<DeveloperInfoAttribute>();
            if (methodAttribute != null)
            {
                methodCount++;
                Console.WriteLine($"\n? Metot #{methodCount}: {method.Name}");
                Console.WriteLine($"   -> Donus Tipi  : {method.ReturnType.Name}");
                
                var parameters = method.GetParameters();
                if (parameters.Length > 0)
                {
                    var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    Console.WriteLine($"   -> Parametreler: {paramList}");
                }
                else
                {
                    Console.WriteLine($"   -> Parametreler: (yok)");
                }
                
                Console.WriteLine($"   -> Yazar       : {methodAttribute.Author}");
                Console.WriteLine($"   -> Versiyon    : {methodAttribute.Version}");
                Console.WriteLine($"   -> Aciklama    : {methodAttribute.Description}");
                Console.WriteLine($"   -> Guncelleme  : {(string.IsNullOrEmpty(methodAttribute.LastModified) ? "-" : methodAttribute.LastModified)}");
            }
        }

        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine($"OZET: {methods.Length} metot tarandi, {methodCount} tanesi DeveloperInfo attribute'una sahip.");
        Console.WriteLine(new string('=', 70) + "\n");
    }
}
