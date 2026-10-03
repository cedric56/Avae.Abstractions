using Avae.ViewModels;
using Example.ViewModels;
using System.ComponentModel;

namespace Example.Maui;

internal class DefaultView : ContentView, IViewFor
{
    public object? Context
    {
        get => BindingContext;
        set
        {
            BindingContext = value;
            if (value is FormViewModel f)
                this.Content = new Label() { Text = "Form" };
            if (value is RegionsViewModel r)
            {
                var view = new ContentView
                {
                    Content = new ActivityIndicator { IsRunning = true }
                };
                _ = WaitForViewsAsync(r, view);                
                this.Content = view;
            }
        }
    }

    private static async Task WaitForViewsAsync(RegionsViewModel vm, ContentView placeholder, CancellationToken ct = default)
    {
        if (vm.MainView is not null && vm.SideView is not null)
            return;

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void Handler(object? s, PropertyChangedEventArgs e)
        {
            if (vm.MainView is not null && vm.SideView is not null)
                tcs.TrySetResult();
        }

        vm.PropertyChanged += Handler;
        try
        {
            if (vm.MainView is not null && vm.SideView is not null)
                return; // re-check after subscribing, avoids a race if both arrived between the first check and the subscribe

            using var reg = ct.Register(() => tcs.TrySetCanceled());
            await tcs.Task.ConfigureAwait(false);
        }
        finally
        {
            vm.PropertyChanged -= Handler;

            var layout = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star },   // MainView fills available space
                    new RowDefinition { Height = GridLength.Auto },   // SideView sized to its content
                    new RowDefinition { Height = GridLength.Star },
                }
            };

            var mainContent = new ContentView();
            mainContent.SetBinding(ContentView.ContentProperty, new Binding(nameof(RegionsViewModel.MainView), source: vm));
            var buttons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Button { Text = "Back", Command = vm.BackCommand },
                    new Button { Text = "Next", Command = vm.NextCommand }
                }
            };
            var sideContent = vm.SideView as View ?? new Label { Text = "Side view not found" };

            Grid.SetRow(mainContent, 0);
            Grid.SetRow(buttons, 1);
            Grid.SetRow(sideContent, 2);

            layout.Children.Add(mainContent);
            layout.Children.Add(buttons);
            layout.Children.Add(sideContent);

            placeholder.Content = layout;
        }
    }
}
