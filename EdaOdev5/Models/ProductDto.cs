using System.ComponentModel.DataAnnotations;

namespace EdaOdev5.Models;

/// <summary>
/// Ürün verilerini temsil eden DTO (Data Transfer Object)
/// Validation attribute'larý ile veri doðrulamasý yapýlýr
/// </summary>
public class ProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ürün adý zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ürün adý 2-100 karakter arasýnda olmalýdýr.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Açýklama en fazla 500 karakter olabilir.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Range(0.01, 1000000, ErrorMessage = "Fiyat 0.01 ile 1.000.000 arasýnda olmalýdýr.")]
    public decimal Price { get; set; }

    [Range(0, 10000, ErrorMessage = "Stok miktarý 0-10.000 arasýnda olmalýdýr.")]
    public int StockQuantity { get; set; }

    [StringLength(50, ErrorMessage = "Kategori en fazla 50 karakter olabilir.")]
    public string? Category { get; set; }
}

/// <summary>
/// Standart hata yanýtý modeli
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
}
