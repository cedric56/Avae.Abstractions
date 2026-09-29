using Avae.Services;

namespace Avae.Maui;

internal class TaskDialogService(IDialogService service) : ITaskDialogService
{
    /// <summary>
    /// Displays a task dialog assembled from header, subheader, content, progress bar, and footer
    /// sections, and awaits the user's button selection.
    /// </summary>
    /// <param name="params">The task dialog's configuration, including header, content, and footer.</param>
    /// <param name="results">Up to three result values corresponding to the primary, secondary, and close buttons.</param>
    /// <returns>The result value associated with the button the user selected.</returns>
    public async Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
    {
        if (results.Count() > 3)
            throw new NotImplementedException("Only three buttons are supported");

        var accent = new AppThemeBinding
        {
            Light = Color.FromArgb("#5B3FD6"),
            Dark = Color.FromArgb("#B39DFF")
        };

        var mainGrid = new Grid
        {
            RowSpacing = 12,
            RowDefinitions =
        {
            new RowDefinition(GridLength.Auto),  // header
            new RowDefinition(GridLength.Auto),  // subheader
            new RowDefinition(GridLength.Star),  // content
        }
        };

        // ===== HEADER: icon + text side by side =====
        var headerHost = new Grid
        {
            ColumnSpacing = 12,
            ColumnDefinitions =
        {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star)
        },
            IsVisible = @params.Header is not null || @params.IconSource is not null
        };

        var iconElement = new Image
        {
            WidthRequest = 32,
            HeightRequest = 32,
            Source = @params.IconSource as ImageSource,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = @params.IconSource is not null,
            AutomationId = "IconElement"
        };
        headerHost.Add(iconElement, 0, 0);

        var headerText = new Label
        {
            Text = @params.Header,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            LineBreakMode = LineBreakMode.WordWrap,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = @params.Header is not null
        };
        headerHost.Add(headerText, 1, 0);
        mainGrid.Add(headerHost, 0, 0);

        // ===== SUBHEADER =====
        var subHeaderText = new Label
        {
            Text = @params.SubHeader,
            FontSize = 14,
            LineBreakMode = LineBreakMode.WordWrap,
            IsVisible = @params.SubHeader is not null
        };
        mainGrid.Add(subHeaderText, 0, 1);

        // ===== CONTENT + PROGRESS + MORE DETAILS =====
        var contentStack = new VerticalStackLayout { Spacing = 18 };

        contentStack.Add(new ContentView
        {
            Content = @params.Content as View,
            HorizontalOptions = LayoutOptions.Fill
        });

        contentStack.Add(new ProgressBar { IsVisible = @params.ShowProgressBar });

        if (@params.Footer is not null)
        {
            var footerHost = new VerticalStackLayout
            {
                Spacing = 12,
                IsVisible = false,
                Children =
            {
                new BoxView { HeightRequest = 1, Opacity = 0.2, Color = Colors.Gray },
                new ContentView { Content = @params.Footer as View }
            }
            };

            var moreDetailsButton = new Button
            {
                Text = "More Details",
                //BackgroundColor = Colors.Transparent,
                BorderWidth = 0,
                Padding = 0,
                HorizontalOptions = LayoutOptions.Start,
                MinimumHeightRequest = 0,
                MinimumWidthRequest = 0,
                FontSize = 14
            };
            moreDetailsButton.SetBinding(Button.TextColorProperty, new Binding { Source = accent });
            moreDetailsButton.Clicked += (_, _) =>
            {
                footerHost.IsVisible = !footerHost.IsVisible;
                moreDetailsButton.Text = footerHost.IsVisible ? "Fewer Details" : "More Details";
            };

            contentStack.Add(moreDetailsButton);
            contentStack.Add(footerHost);
        }

        var scrollView = new ScrollView
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            VerticalScrollBarVisibility = ScrollBarVisibility.Default,
            Content = contentStack
        };
        mainGrid.Add(scrollView, 0, 2);

        return await ((DialogService)service).DisplayThreeButtons<TaskDialogStandardResult>(
            @params.Title,
            mainGrid,
            results.ElementAtOrDefault(0).ToString(),
            results.Length > 1 ? results.ElementAtOrDefault(1).ToString() : null,
            results.Length > 2 ? results.ElementAtOrDefault(2).ToString() : null,
            results.ElementAtOrDefault(0),
            results.ElementAtOrDefault(1),
            results.ElementAtOrDefault(2));
    }
}
