namespace ConsoleApp.Legacy;

/// <summary>
/// Obsolete Attribute kullanýmýný gösteren sýnýf
/// </summary>
public class ObsoleteMethods
{
    /// <summary>
    /// Bu metot kullanýmdan kaldýrýlacak - sadece uyarý verir (isError=false)
    /// </summary>
    [Obsolete("Bu metot kullanýmdan kaldýrýlacaktýr. Lütfen NewCalculation metodunu kullanýnýz.", false)]
    public static int OldCalculation(int a, int b)
    {
        Console.WriteLine("Eski hesaplama metodu çalýþtý (Uyarý veren)");
        return a + b;
    }

    /// <summary>
    /// Bu metot kullanýmdan kaldýrýldý - derleme hatasý verir (isError=true)
    /// NOT: Bu metodu kullanmak derleme hatasý verecektir!
    /// Raporda belirtildiði gibi, bu metodu çaðýran satýr yorum satýrýna alýnmalýdýr.
    /// </summary>
    [Obsolete("Bu metot tamamen kaldýrýlmýþtýr. NewMultiplication metodunu kullanýnýz!", true)]
    public static int OldMultiplication(int a, int b)
    {
        Console.WriteLine("Eski çarpma metodu (Derleme hatasý veren)");
        return a * b;
    }

    /// <summary>
    /// Yeni hesaplama metodu - OldCalculation yerine kullanýlmalý
    /// </summary>
    public static int NewCalculation(int a, int b)
    {
        Console.WriteLine("Yeni hesaplama metodu çalýþtý");
        return a + b;
    }

    /// <summary>
    /// Yeni çarpma metodu - OldMultiplication yerine kullanýlmalý
    /// </summary>
    public static int NewMultiplication(int a, int b)
    {
        Console.WriteLine("Yeni çarpma metodu çalýþtý");
        return a * b;
    }
}
