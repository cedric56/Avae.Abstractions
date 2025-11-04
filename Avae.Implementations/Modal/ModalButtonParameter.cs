using CommunityToolkit.Mvvm.Input;

namespace Avae.Implementations;

public class ModalButtonParameter
{
    public RelayCommand? Command { get; set; }

    public object? Value { get; set; }

    public bool IsDefault { get; set; }
}
