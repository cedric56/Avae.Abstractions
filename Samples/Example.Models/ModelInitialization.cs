using Dommel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Example.Models;

public static class ModelInitialization
{
#pragma warning disable CA2255 // L’attribut ’ModuleInitializer’ ne doit pas être utilisé dans les bibliothèques
    [ModuleInitializer]
#pragma warning restore CA2255 // L’attribut ’ModuleInitializer’ ne doit pas être utilisé dans les bibliothèques
    public static void Init()
    {
        DommelMapper.SetPropertyResolver(new DommelPropertyResolver());
    }
}

internal class DommelPropertyResolver : DefaultPropertyResolver
{
    public override IEnumerable<ColumnPropertyInfo> ResolveProperties(Type type)
    {
        var properties = base.ResolveProperties(type);
        foreach (var propertyInfo in properties)
        {
            var notMappedAttr = propertyInfo.Property.GetCustomAttribute<NotMappedAttribute>();

            if (notMappedAttr == null)
            {
                yield return propertyInfo;
            }
        }
    }
}