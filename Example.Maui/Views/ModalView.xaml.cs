using Avae.Abstractions;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using Example.ViewModels;

namespace Example.Maui.Views;

public abstract class DialogViewBase : ContentView
{ }

public class DialogView<TViewModel, TResult>(ICurrentPage provider) : DialogViewBase,
    IModalFor<TViewModel, TResult>
    where TViewModel : class, ICloseableViewModel<TResult> 
{
    public object? Context { get => BindingContext; set => BindingContext = value; }

    TaskCompletionSource<TResult?>? Tcs = null;

    public async Task<TResult?> ShowModalAsync()
    {
        Tcs = new TaskCompletionSource<TResult?>();

        var wm = BindingContext as TViewModel;
        if (wm != null)
        {
            wm.CloseRequested += (s, e) =>
            {
                Tcs.TrySetResult(e);
            };
        }
        await provider.Current.ShowPopupAsync(this, new PopupOptions()
        {
             CanBeDismissedByTappingOutsideOfPopup = false
        });
        //await Navigation.PushModalAsync(new ContentPage() { Content = this });
        var result = await Tcs.Task;
        return result;
    }
}

public partial class ModalView : DialogView<ModalViewModel, string>
{
	public ModalView(ICurrentPage provider)
        : base(provider)
    {
		InitializeComponent();
	}
}