using EdaOdev5.Middleware;
using EdaOdev5.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Filter'larý DI container'a kaydet
builder.Services.AddScoped<ExecutionTimingFilter>();
builder.Services.AddScoped<GlobalExceptionFilter>();

// Controller'larý filter'larla birlikte ekle
builder.Services.AddControllers(options =>
{
    // Global olarak tüm action'lara uygulanan filter'lar
    options.Filters.Add<ExecutionTimingFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "EdaOdev5 API",
        Version = "v1",
        Description = "ÖDEV 09 - C# Temelleri, Reflection ve ASP.NET Core Web API Entegrasyonu"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Custom Request Logging Middleware - Pipeline'ýn en baþýnda olmalý
app.UseRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Uygulama baþlangýç mesajý
Console.WriteLine("???????????????????????????????????????????????????????????????????????");
Console.WriteLine("   EdaOdev5 API Baþlatýldý!");
Console.WriteLine("   Swagger UI: https://localhost:{port}/swagger");
Console.WriteLine("   API Endpoints:");
Console.WriteLine("     - GET  /api/products         : Tüm ürünleri listele");
Console.WriteLine("     - GET  /api/products/{id}    : ID ile ürün getir");
Console.WriteLine("     - POST /api/products         : Yeni ürün ekle");
Console.WriteLine("     - PUT  /api/products/{id}    : Ürün güncelle");
Console.WriteLine("     - DELETE /api/products/{id}  : Ürün sil");
Console.WriteLine("     - GET  /api/system/attribute-map : API metadata haritasý");
Console.WriteLine("???????????????????????????????????????????????????????????????????????");

app.Run();
