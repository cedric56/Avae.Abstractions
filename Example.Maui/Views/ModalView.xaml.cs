using Avae.Abstractions;
using Avae.DAL;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Example.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using System.Globalization;

namespace Example.Maui.Views;

public class DialogViewBase<TViewModel, TResult> : Popup<TResult?>
     where TViewModel : class, ICloseableViewModel<TResult>
{ 
    public DialogViewBase(DialogView<TViewModel, TResult> view)
    {
        

        // Main layout
        var mainLayout = new VerticalStackLayout
        {
            Spacing = 0
        };

        // === Dialog Title ===
        var titleBorder = new Border
        {
            StrokeThickness = 0,
            
            //Padding = new Thickness(20, 15, 20, 15),
            HorizontalOptions = LayoutOptions.Fill
        };

        var titleLabel = new Label
        {
            Text = view.Title,
            TextColor = Colors.White,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center
        };

        titleBorder.Content = titleLabel;
        mainLayout.Children.Add(titleBorder);

        // === Content Area ===
        var contentLayout = new VerticalStackLayout
        {
            Padding = new Thickness(20, 20, 20, 20),
            //Spacing = 15
        };

        contentLayout.Children.Add(view);

        mainLayout.Children.Add(contentLayout);

        // === Button Area ===        

        var buttonLayout = new HorizontalStackLayout
        {
            BackgroundColor = Color.FromHex("#4D000000"),
            //Padding = new Thickness(15, 12, 15, 12),
            Spacing = 8,
            HorizontalOptions = LayoutOptions.Fill
        };

        var collection = new CollectionView() {
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
            {
                ItemSpacing = 10
            },
            HorizontalOptions = LayoutOptions.End,
        };
        var c = new IndexConverter();
        collection.SetBinding(CollectionView.ItemsSourceProperty, new Binding("Commands"));
        collection.ItemTemplate = new DataTemplate(() =>
        {
            var button = new Button();
            button.SetBinding(Button.TextProperty, new Binding("Index") { Converter = c, ConverterParameter = view.Buttons.Split(",") });
            button.SetBinding(Button.CommandProperty, new Binding("Command"));
            return button;
        });
        buttonLayout.Add(collection);

        mainLayout.Children.Add(buttonLayout);


        Content = mainLayout;
    }

    class IndexConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return parameter is string[] names && value is int index && index >= 0 && index < names.Length
                ? names[index]
                : null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

public abstract class DialogView<TViewModel, TResult>(ICurrentPage provider) : ContentView,
    IModalFor<TViewModel, TResult>
    where TViewModel : class, ICloseableViewModel<TResult> 
{
    abstract public string Title { get; }

    abstract public string Buttons { get; }

    public object? Context { get => BindingContext; set => BindingContext = value; }
    
    public async Task<TResult?> ShowModalAsync()
    {
        var currentTheme = Application.Current?.UserAppTheme ?? AppTheme.Light;

        // Define theme-aware colors
        Color overlayColor = currentTheme == AppTheme.Light
            ? Colors.Black.WithAlpha(0.5f)   // Lighter overlay for light mode
            : Colors.Black.WithAlpha(0.7f);  // Darker overlay for dark mode

        Color popupBackgroundColor = currentTheme == AppTheme.Light
            ? Colors.White
            : Color.FromArgb("#FF1E1E1E");  // Dark background for dark mode

        var wm = BindingContext as TViewModel ?? throw new InvalidOperationException("BindingContext is not of type TViewModel");        
        var pop = new DialogViewBase<TViewModel, TResult>(this) { BindingContext = wm };
        pop.BackgroundColor = popupBackgroundColor;
        wm.CloseRequested += CloseRequestedHandler;
        var result = await provider.Current.ShowPopupAsync<TResult?>(
            pop, new PopupOptions()
            {                 
                CanBeDismissedByTappingOutsideOfPopup = false,
                PageOverlayColor = overlayColor,                 
                Shape = new RoundRectangle { CornerRadius = new CornerRadius(8) }

            });
        return result.Result;

        async void CloseRequestedHandler(object? sender, TResult? e)
        {
            wm.CloseRequested -= CloseRequestedHandler;
            await pop.CloseAsync(e);
        }
    }
}

public partial class ModalView : DialogView<ModalViewModel, string?>
{
	public ModalView(ICurrentPage provider)
        : base(provider)
    {
		InitializeComponent();
	}

    public override string Title => "Modal";

    public override string Buttons => "Ok,Cancel";
}