using Avae.ViewModels;
using MauiIcons.Core;
using MauiIcons.FontAwesome.Solid;

namespace Example.Maui;

internal class ExampleIconResolver : IIconResolver
{
    public object? GetIcon(string path)
    {
        if (path.StartsWith("fa-solid fa-"))
        {
            var name = path["fa-solid fa-".Length..].Replace("-", "");
            if (Enum.TryParse<FontAwesomeSolidIcons>(name, true, out var icon))
            {
                return icon;
            }
        }
        return null;
    }

    public object? GetSource(string key)
    {
        if (key.StartsWith("fa-solid fa-"))
        {
            var name = key["fa-solid fa-".Length..].Replace("-", "");
            if (Enum.TryParse<FontAwesomeSolidIcons>(name, true, out var icon))
            {
                return icon.ToImageSource();
            }
        }
        return null;
    }
}
