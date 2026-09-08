using Avae.Services;

namespace Avae.Maui.Services;

internal class TaskDialogService(Helper helper) : ITaskDialogService
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
        // Main Grid
        var mainGrid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
        {
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Star },
            new RowDefinition { Height = GridLength.Auto }
        }
        };

        // ============ HEADER HOST ============
        var headerHost = new Grid
        {
            IsVisible = @params.Header is not null || @params.IconSource is not null
        };
        Grid.SetRow(headerHost, 0);

        // Icon Host (Viewbox)
        //var iconHost = new Viewbox
        //{
        //    WidthRequest = (double)Application.Current.Resources["TaskDialogIconSize"],
        //    HeightRequest = (double)Application.Current.Resources["TaskDialogIconSize"],
        //    VerticalOptions = LayoutOptions.Center,
        //    HorizontalOptions = LayoutOptions.Start,
        //    Margin = (Thickness)Application.Current.Resources["TaskDialogIconMargin"],
        //    IsVisible = false
        //};

        var iconElement = new Image
        {
            Source = @params.IconSource as ImageSource,
            AutomationId = "IconElement"
        };
        //iconHost.Child = iconElement;

        // Header Text
        var headerText = new Label
        {
            Text = @params.Header,
            LineBreakMode = LineBreakMode.WordWrap,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            IsVisible = @params.Header is not null
        };

        headerHost.Children.Add(iconElement);
        headerHost.Children.Add(headerText);
        mainGrid.Children.Add(headerHost);

        // ============ SUBHEADER TEXT ============
        var subHeaderText = new Label
        {
            Text = @params.SubHeader,
            FontSize = 9,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.WordWrap,
            IsVisible = @params.SubHeader is not null
        };
        Grid.SetRow(subHeaderText, 1);
        mainGrid.Children.Add(subHeaderText);

        // ============ CONTENT AREA ============
        var scrollView = new ScrollView
        {
            //Margin = (Thickness)Application.Current.Resources["TaskDialogContentMargin"],
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            VerticalScrollBarVisibility = ScrollBarVisibility.Default
        };
        Grid.SetRow(scrollView, 2);

        var contentStack = new StackLayout
        {
            //Spacing = 18
        };

        // Content Presenter
        var contentPresenter = new Microsoft.Maui.Controls.ContentView
        {
            Content = @params.Content as Microsoft.Maui.Controls.View,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        contentStack.Children.Add(contentPresenter);

        // Progress Bar
        var progressBar = new ProgressBar
        {
            IsVisible = @params.ShowProgressBar
        };
        contentStack.Children.Add(progressBar);

        // More Details Panel
        var moreDetailsPanel = new StackLayout
        {
            IsVisible = false,
            Spacing = 0
        };

        var moreDetailsButton = new Button
        {
            Text = "Footer",// (string)Application.Current.Resources["TaskDialogFooterButtonNormalText"],
            IsVisible = @params.Footer is not null,
            //Style = (Style)Application.Current.Resources["TaskDialogMoreDetailsButton"]
        };
        moreDetailsPanel.Children.Add(moreDetailsButton);

        var footerHost = new Microsoft.Maui.Controls.ContentView
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill,
            Content = @params.Footer as Microsoft.Maui.Controls.View,
            IsVisible = @params.Footer is not null
        };
        moreDetailsPanel.Children.Add(footerHost);

        contentStack.Children.Add(moreDetailsPanel);
        scrollView.Content = contentStack;
        mainGrid.Children.Add(scrollView);

        // Return or set the main grid as your content
        // this.Content = mainGrid;

        return await helper.DisplayThreeButtons<TaskDialogStandardResult>(
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
