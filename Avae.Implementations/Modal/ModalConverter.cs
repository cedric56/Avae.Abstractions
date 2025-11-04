using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.Input;

namespace Avae.Implementations;

public class ModalConverter : IMultiValueConverter
{
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        bool isDefault = false;
        if (values[2] is bool @default)
            isDefault = @default;


        var @params = new ModalButtonParameter
        {
            Command = values[0] as RelayCommand,
            Value = values[1],
            IsDefault = isDefault
        };
        return @params;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
