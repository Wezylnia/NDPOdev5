using Microsoft.AspNetCore.Mvc;
using EdaOdev5.Models;

namespace EdaOdev5.Controllers;

/// <summary>
/// Ürün CRUD iþlemlerini yöneten API Controller
/// Attribute-based routing kullanýlmaktadýr
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Basit in-memory ürün listesi (gerçek projede veritabaný kullanýlýr)
    private static List<ProductDto> _products = new()
    {
        new ProductDto { Id = 1, Name = "Laptop", Description = "Gaming Laptop", Price = 25000, StockQuantity = 10, Category = "Elektronik" },
        new ProductDto { Id = 2, Name = "Telefon", Description = "Akýllý Telefon", Price = 15000, StockQuantity = 25, Category = "Elektronik" },
        new ProductDto { Id = 3, Name = "Kulaklýk", Description = "Bluetooth Kulaklýk", Price = 500, StockQuantity = 100, Category = "Aksesuar" }
    };
    private static int _nextId = 4;

    /// <summary>
    /// Tüm ürünleri listeler
    /// GET: api/products
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<ProductDto>> GetAll()
    {
        return Ok(_products);
    }

    /// <summary>
    /// ID'ye göre ürün getirir
    /// GET: api/products/{id}
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<ProductDto> GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        
        if (product == null)
        {
            return NotFound(new ErrorResponse
            {
                StatusCode = 404,
                Message = "Ürün bulunamadý",
                Details = $"ID: {id} ile eþleþen ürün mevcut deðil."
            });
        }

        return Ok(product);
    }

    /// <summary>
    /// Yeni ürün ekler
    /// POST: api/products
    /// </summary>
    [HttpPost]
    public ActionResult<ProductDto> Create([FromBody] ProductDto productDto)
    {
        // Model validation otomatik olarak yapýlýr
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        productDto.Id = _nextId++;
        _products.Add(productDto);

        return CreatedAtAction(nameof(GetById), new { id = productDto.Id }, productDto);
    }

    /// <summary>
    /// Mevcut ürünü günceller
    /// PUT: api/products/{id}
    /// </summary>
    [HttpPut("{id}")]
    public ActionResult<ProductDto> Update(int id, [FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingProduct = _products.FirstOrDefault(p => p.Id == id);
        
        if (existingProduct == null)
        {
            return NotFound(new ErrorResponse
            {
                StatusCode = 404,
                Message = "Güncellenecek ürün bulunamadý",
                Details = $"ID: {id} ile eþleþen ürün mevcut deðil."
            });
        }

        existingProduct.Name = productDto.Name;
        existingProduct.Description = productDto.Description;
        existingProduct.Price = productDto.Price;
        existingProduct.StockQuantity = productDto.StockQuantity;
        existingProduct.Category = productDto.Category;

        return Ok(existingProduct);
    }

    /// <summary>
    /// Ürünü siler
    /// DELETE: api/products/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        
        if (product == null)
        {
            return NotFound(new ErrorResponse
            {
                StatusCode = 404,
                Message = "Silinecek ürün bulunamadý",
                Details = $"ID: {id} ile eþleþen ürün mevcut deðil."
            });
        }

        _products.Remove(product);
        return NoContent();
    }

    /// <summary>
    /// Kategoriye göre ürünleri filtreler
    /// GET: api/products/category/{category}
    /// </summary>
    [HttpGet("category/{category}")]
    public ActionResult<IEnumerable<ProductDto>> GetByCategory(string category)
    {
        var products = _products.Where(p => 
            p.Category != null && 
            p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

        return Ok(products);
    }
}
