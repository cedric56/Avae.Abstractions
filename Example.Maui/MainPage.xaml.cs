using Avae.Abstractions;
using Example.ViewModels;

namespace Example.Maui
{
    public partial class MainPage : FlyoutEx, IContextFor<MainViewModel>
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public object? Context { get => BindingContext; set => BindingContext = value; }
    }

    public class FlyoutEx : FlyoutPage
    {
        public static readonly BindableProperty CurrentPageProperty =
  BindableProperty.Create("CurrentPage", typeof(ContentPage), typeof(FlyoutEx), null, propertyChanged: OnCurrentPageChanged);

        static void OnCurrentPageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var f = (FlyoutPage)bindable;
            f.Detail = newValue as ContentPage;
        }

        public ContentPage CurrentPage
        {
            get => (ContentPage)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

    }
}
