using ConsoleApp.Models;
using ConsoleApp.Services;
using ConsoleApp.Legacy;
using ConsoleApp.Helpers;

namespace ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Console.WriteLine("======================================================================");
        Console.WriteLine("     ODEV 09 - Console Uygulamasi: C# Temelleri ve Reflection");
        Console.WriteLine("======================================================================\n");

        // BÖLÜM 1.1: Struct ve Değer Tipleri
        DemonstrateStructsAndValueTypes();

        // BÖLÜM 1.2: Exception Handling
        DemonstrateExceptionHandling();

        // BÖLÜM 1.3: Obsolete Attribute Kullanımı
        DemonstrateObsoleteAttribute();

        // BÖLÜM 1.4: Custom Attribute ve Reflection Raporu
        DemonstrateCustomAttributeAndReflection();

        Console.WriteLine("\n\nProgram sonlandi. Cikmak icin bir tusa basiniz...");
        Console.ReadKey();
    }

    /// <summary>
    /// BÖLÜM 1.1: Struct yapılarının değer tipi olarak davranışını gösterir
    /// </summary>
    static void DemonstrateStructsAndValueTypes()
    {
        Console.WriteLine("\n" + new string('-', 70));
        Console.WriteLine("BOLUM 1.1: STRUCT VE DEGER TIPLERI");
        Console.WriteLine(new string('-', 70));

        // 3 farklı Student struct örneği oluştur
        var students = new List<Student>
        {
            new Student(1, "Ahmet Yılmaz", 3.45),
            new Student(2, "Ayşe Demir", 3.78),
            new Student(3, "Mehmet Kaya", 2.95)
        };

        Console.WriteLine("\nOgrenci Listesi:");
        Console.WriteLine(new string('-', 50));
        foreach (var student in students)
        {
            Console.WriteLine($"   {student}");
        }

        // Deger tipi davranisini goster
        Console.WriteLine("\nDeger Tipi (Value Type) Davranisi:");
        Console.WriteLine(new string('-', 50));
        
        Student original = new Student(4, "Zeynep Ak", 3.90);
        Student copy = original; // Değer kopyalanır, referans değil
        
        Console.WriteLine($"   Orijinal: {original}");
        Console.WriteLine($"   Kopya   : {copy}");
        
        // Kopyayı değiştir
        copy.Name = "Zeynep Ak (Değiştirildi)";
        copy.Gpa = 4.00;
        
        Console.WriteLine("\n   [Kopya degistirildi: Name ve Gpa guncellendi]");
        Console.WriteLine($"   Orijinal: {original}");
        Console.WriteLine($"   Kopya   : {copy}");
        Console.WriteLine("\n   -> Gordugunuz gibi, kopya degisse de orijinal degismedi!");
        Console.WriteLine("   -> Bu, struct'larin deger tipi oldugunu kanitlar.");
    }

    /// <summary>
    /// BÖLÜM 1.2: Exception Handling - try/catch/finally kullanımı
    /// </summary>
    static void DemonstrateExceptionHandling()
    {
        Console.WriteLine("\n\n" + new string('-', 70));
        Console.WriteLine("BOLUM 1.2: EXCEPTION HANDLING (HATA YONETIMI)");
        Console.WriteLine(new string('-', 70));

        // Test 1: Basarili islem
        Console.WriteLine("\nTest 1: Basarili Bolme Islemi");
        PerformDivision("20", "4");

        // Test 2: DivideByZeroException
        Console.WriteLine("\nTest 2: Sifira Bolme Hatasi (DivideByZeroException)");
        PerformDivision("15", "0");

        // Test 3: FormatException
        Console.WriteLine("\nTest 3: Format Hatasi (FormatException)");
        PerformDivision("abc", "5");
    }

    /// <summary>
    /// Bölme işlemi yapan ve hataları yakalayan metot
    /// </summary>
    static void PerformDivision(string num1Str, string num2Str)
    {
        Console.WriteLine($"   Girdi: {num1Str} / {num2Str}");
        
        try
        {
            int num1 = int.Parse(num1Str); // FormatException olabilir
            int num2 = int.Parse(num2Str); // FormatException olabilir
            
            int result = num1 / num2; // DivideByZeroException olabilir
            
            Console.WriteLine($"   [OK] Sonuc: {num1} / {num2} = {result}");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"   [HATA] DivideByZeroException yakalandi!");
            Console.WriteLine($"      Mesaj: {ex.Message}");
            Console.WriteLine($"      Aciklama: Sifira bolme islemi yapilamaz.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"   [HATA] FormatException yakalandi!");
            Console.WriteLine($"      Mesaj: {ex.Message}");
            Console.WriteLine($"      Aciklama: Girilen deger gecerli bir sayi degil.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   [HATA] Beklenmeyen hata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"   [FINALLY] Islem tamamlandi (hata olsa da olmasa da)");
        }
    }

    /// <summary>
    /// BÖLÜM 1.3: Obsolete Attribute kullanımını gösterir
    /// </summary>
    static void DemonstrateObsoleteAttribute()
    {
        Console.WriteLine("\n\n" + new string('-', 70));
        Console.WriteLine("BOLUM 1.3: OBSOLETE ATTRIBUTE KULLANIMI");
        Console.WriteLine(new string('-', 70));

        Console.WriteLine("\n? Obsolete(isError=false) - Uyari veren metot:");
        Console.WriteLine("   Derleme sirasinda uyari verir ama calisir.");
        
        // Bu satır uyarı verecek ama çalışacak
        #pragma warning disable CS0618 // Obsolete uyarısını göster ama derlemeye izin ver
        int result1 = ObsoleteMethods.OldCalculation(5, 3);
        #pragma warning restore CS0618
        Console.WriteLine($"   Sonuç: 5 + 3 = {result1}");

        Console.WriteLine("\n? Obsolete(isError=true) - Derleme hatasi veren metot:");
        Console.WriteLine("   Bu metot cagrildiginda derleme HATASI verir!");
        Console.WriteLine("   Asagidaki satir yorum satirina alinmistir:");
        Console.WriteLine("   // int result2 = ObsoleteMethods.OldMultiplication(5, 3);");
        
        // DERLEME HATASI VERECEGI ICIN YORUM SATIRINA ALINMISTIR:
        // int result2 = ObsoleteMethods.OldMultiplication(5, 3);

        Console.WriteLine("\n? Yeni metotlarin kullanimi (onerilen):");
        int result3 = ObsoleteMethods.NewCalculation(5, 3);
        Console.WriteLine($"   NewCalculation: 5 + 3 = {result3}");
        
        int result4 = ObsoleteMethods.NewMultiplication(5, 3);
        Console.WriteLine($"   NewMultiplication: 5 * 3 = {result4}");
    }

    /// <summary>
    /// BÖLÜM 1.4: Custom Attribute ve Reflection kullanarak metadata raporu
    /// </summary>
    static void DemonstrateCustomAttributeAndReflection()
    {
        Console.WriteLine("\n\n" + new string('-', 70));
        Console.WriteLine("BOLUM 1.4: CUSTOM ATTRIBUTE VE REFLECTION RAPORU");
        Console.WriteLine(new string('-', 70));

        Console.WriteLine("\nSampleService sinifi uzerinde Reflection analizi yapiliyor...");
        
        // SampleService tipini al ve Reflection raporu oluştur
        Type sampleServiceType = typeof(SampleService);
        ReflectionHelper.GenerateAttributeReport(sampleServiceType);

        // Servisi calistir
        Console.WriteLine("SampleService metotlari calistiriliyor:");
        var service = new SampleService();
        service.GetAllItems();
        service.GetItemById(1);
        service.AddItem("Test Ürünü");
        service.DeleteItem(1);
    }
}
