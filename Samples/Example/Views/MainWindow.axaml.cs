using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;

namespace Example.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendClientAreaToDecorationsHint = true;
        this.PointerPressed += (s, e) =>
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                if (e.Source is Border bdr && bdr.Name == "PART_TopBar")
                    this.BeginMoveDrag(e);
            }
        };

    }
}
