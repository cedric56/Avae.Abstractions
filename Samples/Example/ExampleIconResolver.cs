using Avae.ViewModels;
using Optris.Icons.Avalonia;
using Optris.Icons.Avalonia.FontAwesome;
using System;

namespace Example;

internal class ExampleIconResolver : IIconResolver
{
    static ExampleIconResolver()
    {
        IconProvider.Current.Register<FontAwesomeIconProvider>();
    }

    public object? GetIcon(string key)
    {
        return new Icon() { Value = key };
    }

    public object? GetSource(string key)
    {
        throw new NotImplementedException();
    }
}
