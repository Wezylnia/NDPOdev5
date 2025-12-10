using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace EdaOdev5.Controllers;

/// <summary>
/// API'nin metadata haritasýný Reflection kullanarak çýkaran controller
/// Projedeki tüm controller ve action'larý analiz eder
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    /// <summary>
    /// API'nin attribute haritasýný döndürür
    /// GET: api/system/attribute-map
    /// 
    /// Bu endpoint:
    /// - Projedeki tüm Controller'larý Reflection ile bulur
    /// - Her controller'ýn içindeki Action (metot) listesini çýkarýr
    /// - Action'larýn üzerinde hangi HTTP attribute'larýnýn kullanýldýðýný analiz eder
    /// </summary>
    [HttpGet("attribute-map")]
    public ActionResult<ApiMetadataMap> GetAttributeMap()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var metadataMap = new ApiMetadataMap
        {
            AssemblyName = assembly.GetName().Name ?? "Unknown",
            GeneratedAt = DateTime.UtcNow,
            Controllers = new List<ControllerMetadata>()
        };

        // Controller sýnýflarýný bul (ControllerBase'den türeyen veya [ApiController] attribute'u olan)
        var controllerTypes = assembly.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && 
                           !type.IsAbstract &&
                           type.IsClass)
            .ToList();

        foreach (var controllerType in controllerTypes)
        {
            var controllerMetadata = AnalyzeController(controllerType);
            metadataMap.Controllers.Add(controllerMetadata);
        }

        metadataMap.TotalControllers = metadataMap.Controllers.Count;
        metadataMap.TotalActions = metadataMap.Controllers.Sum(c => c.Actions.Count);

        return Ok(metadataMap);
    }

    /// <summary>
    /// Tek bir controller'ý analiz eder
    /// </summary>
    private ControllerMetadata AnalyzeController(Type controllerType)
    {
        var metadata = new ControllerMetadata
        {
            Name = controllerType.Name,
            FullName = controllerType.FullName ?? controllerType.Name,
            Namespace = controllerType.Namespace ?? "Unknown",
            Actions = new List<ActionMetadata>()
        };

        // Controller-level attribute'larý analiz et
        metadata.ControllerAttributes = GetAttributeInfo(controllerType.GetCustomAttributes(true));

        // Route attribute'unu bul
        var routeAttr = controllerType.GetCustomAttribute<RouteAttribute>();
        metadata.RouteTemplate = routeAttr?.Template ?? "[controller]";

        // Public instance metotlarý (Action'larý) bul
        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var method in methods)
        {
            // Sadece HTTP attribute'u olan metotlarý al (bunlar action'dýr)
            if (IsActionMethod(method))
            {
                var actionMetadata = AnalyzeAction(method);
                metadata.Actions.Add(actionMetadata);
            }
        }

        return metadata;
    }

    /// <summary>
    /// Bir metotun action olup olmadýðýný kontrol eder
    /// </summary>
    private bool IsActionMethod(MethodInfo method)
    {
        // HTTP method attribute'u varsa veya public IActionResult/ActionResult dönen metotsa action'dýr
        var httpAttributes = new[]
        {
            typeof(HttpGetAttribute),
            typeof(HttpPostAttribute),
            typeof(HttpPutAttribute),
            typeof(HttpDeleteAttribute),
            typeof(HttpPatchAttribute),
            typeof(HttpHeadAttribute),
            typeof(HttpOptionsAttribute)
        };

        return httpAttributes.Any(attr => method.GetCustomAttribute(attr) != null) ||
               (method.ReturnType.IsAssignableTo(typeof(IActionResult)) ||
                method.ReturnType.Name.StartsWith("ActionResult") ||
                method.ReturnType.Name.StartsWith("Task"));
    }

    /// <summary>
    /// Tek bir action metotunu analiz eder
    /// </summary>
    private ActionMetadata AnalyzeAction(MethodInfo method)
    {
        var actionMetadata = new ActionMetadata
        {
            Name = method.Name,
            ReturnType = GetFriendlyTypeName(method.ReturnType),
            Parameters = new List<ParameterMetadata>(),
            HttpMethods = new List<string>(),
            Attributes = new List<AttributeInfo>()
        };

        // HTTP metotlarýný belirle
        if (method.GetCustomAttribute<HttpGetAttribute>() is HttpGetAttribute getAttr)
        {
            actionMetadata.HttpMethods.Add("GET");
            actionMetadata.RouteTemplate = getAttr.Template;
        }
        if (method.GetCustomAttribute<HttpPostAttribute>() is HttpPostAttribute postAttr)
        {
            actionMetadata.HttpMethods.Add("POST");
            actionMetadata.RouteTemplate ??= postAttr.Template;
        }
        if (method.GetCustomAttribute<HttpPutAttribute>() is HttpPutAttribute putAttr)
        {
            actionMetadata.HttpMethods.Add("PUT");
            actionMetadata.RouteTemplate ??= putAttr.Template;
        }
        if (method.GetCustomAttribute<HttpDeleteAttribute>() is HttpDeleteAttribute deleteAttr)
        {
            actionMetadata.HttpMethods.Add("DELETE");
            actionMetadata.RouteTemplate ??= deleteAttr.Template;
        }
        if (method.GetCustomAttribute<HttpPatchAttribute>() is HttpPatchAttribute patchAttr)
        {
            actionMetadata.HttpMethods.Add("PATCH");
            actionMetadata.RouteTemplate ??= patchAttr.Template;
        }

        // Diðer önemli attribute'larý ekle
        var allAttributes = method.GetCustomAttributes(true);
        actionMetadata.Attributes = GetAttributeInfo(allAttributes);

        // Parametreleri analiz et
        foreach (var param in method.GetParameters())
        {
            actionMetadata.Parameters.Add(new ParameterMetadata
            {
                Name = param.Name ?? "unknown",
                Type = GetFriendlyTypeName(param.ParameterType),
                IsOptional = param.IsOptional,
                DefaultValue = param.DefaultValue?.ToString(),
                Attributes = GetAttributeInfo(param.GetCustomAttributes(true))
            });
        }

        return actionMetadata;
    }

    /// <summary>
    /// Attribute'larý bilgi listesine dönüþtürür
    /// </summary>
    private List<AttributeInfo> GetAttributeInfo(object[] attributes)
    {
        var result = new List<AttributeInfo>();
        
        foreach (var attr in attributes)
        {
            var attrType = attr.GetType();
            
            // Sistem attribute'larýný filtrele
            if (attrType.Namespace?.StartsWith("System.Runtime") == true ||
                attrType.Namespace?.StartsWith("System.Diagnostics") == true)
            {
                continue;
            }

            var attrInfo = new AttributeInfo
            {
                Name = attrType.Name.Replace("Attribute", ""),
                FullName = attrType.FullName ?? attrType.Name,
                Properties = new Dictionary<string, string?>()
            };

            // Attribute'un public property'lerini oku
            foreach (var prop in attrType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    if (prop.CanRead)
                    {
                        var value = prop.GetValue(attr);
                        attrInfo.Properties[prop.Name] = value?.ToString();
                    }
                }
                catch
                {
                    // Property okunamýyorsa atla
                }
            }

            result.Add(attrInfo);
        }

        return result;
    }

    /// <summary>
    /// Type adýný okunabilir formata çevirir
    /// </summary>
    private string GetFriendlyTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeName = type.Name.Split('`')[0];
            var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
            return $"{genericTypeName}<{genericArgs}>";
        }

        return type.Name;
    }
}

#region Metadata Modelleri

/// <summary>
/// API metadata haritasý ana modeli
/// </summary>
public class ApiMetadataMap
{
    public string AssemblyName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public int TotalControllers { get; set; }
    public int TotalActions { get; set; }
    public List<ControllerMetadata> Controllers { get; set; } = new();
}

/// <summary>
/// Controller metadata bilgisi
/// </summary>
public class ControllerMetadata
{
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string? RouteTemplate { get; set; }
    public List<AttributeInfo> ControllerAttributes { get; set; } = new();
    public List<ActionMetadata> Actions { get; set; } = new();
}

/// <summary>
/// Action (metot) metadata bilgisi
/// </summary>
public class ActionMetadata
{
    public string Name { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public string? RouteTemplate { get; set; }
    public List<string> HttpMethods { get; set; } = new();
    public List<ParameterMetadata> Parameters { get; set; } = new();
    public List<AttributeInfo> Attributes { get; set; } = new();
}

/// <summary>
/// Parametre metadata bilgisi
/// </summary>
public class ParameterMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsOptional { get; set; }
    public string? DefaultValue { get; set; }
    public List<AttributeInfo> Attributes { get; set; } = new();
}

/// <summary>
/// Attribute bilgisi
/// </summary>
public class AttributeInfo
{
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Dictionary<string, string?> Properties { get; set; } = new();
}

#endregion
