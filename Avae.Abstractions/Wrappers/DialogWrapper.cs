using System.Text;

namespace Avae.Abstractions
{
    //public static class DialogWrapper
    //{        

    //    //private static string ToFullBlownString(Exception e, int level = int.MaxValue)
    //    //{
    //    //    var sb = new StringBuilder();
    //    //    var exception = e;
    //    //    var counter = 1;
    //    //    while (exception != null && counter <= level)
    //    //    {
    //    //        //var stackFrame = (new StackTrace(exception, true)).GetFrame(0);
    //    //        //var message = string.Format("At line {0} column {1} in {2}: {3} {4}{3}{5}  ",
    //    //        //   stackFrame.GetFileLineNumber(), stackFrame.GetFileColumnNumber(),
    //    //        //   stackFrame.GetMethod(), Environment.NewLine, stackFrame.GetFileName(),
    //    //        //   exception.Message);

    //    //        sb.AppendLine($"{counter}-> Level: {counter}");
    //    //        sb.AppendLine($"{counter}-> Message: {exception.Message}");
    //    //        sb.AppendLine($"{counter}-> Source: {exception.Source}");
    //    //        sb.AppendLine($"{counter}-> Target Site: {exception.TargetSite}");
    //    //        sb.AppendLine($"{counter}-> Stack Trace: {exception.StackTrace}");
    //    //        //sb.AppendLine($"{counter}-> Formatted: {message}");

    //    //        exception = exception.InnerException;
    //    //        counter++;
    //    //    }

    //    //    return sb.ToString();
    //    //}

    //    //public static IDialogService Instance => SimpleProvider.GetService<IDialogService>();

    //    //public static async Task ShowErrorAsync(Exception ex, string title = "Error")
    //    //{
    //    //    var message = ToFullBlownString(ex);
    //    //    await Instance.ShowErrorAsync(ex, title);
    //    //}
    //    //public static async Task ShowOkAsync(string message, string title = "Title")
    //    //{
    //    //    await Instance.ShowOkAsync(message, title);
    //    //}

    //    //public static async Task<bool> ShowYesNoAsync(string message, string title = "Title")
    //    //{
    //    //    return await Instance.ShowYesNoAsync(message, title);
    //    //}

    //    //public static async Task<bool> ShowOkCancelAsync(string message, string title = "Title")
    //    //{
    //    //    return await Instance.ShowOkCancelAsync(message, title);
    //    //}

    //    //public static async Task<bool> ShowOkAbortAsync(string message, string title = "Title")
    //    //{
    //    //    return await Instance.ShowOkAbortAsync(message, title);
    //    //}

    //    //public static Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
    //    //{
    //    //    return Instance.ShowYesNoCancelAsync(message, title);
    //    //}

    //    //public static Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
    //    //{
    //    //    return Instance.ShowYesNoAbortAsync(message, title);
    //    //}

    //    public static Task<TResult?> ShowDialogAsync<TViewModel, TResult>(this IServiceProvider provider, params IParameter[] parameters) where TViewModel : class, IViewModelBase
    //    {
    //        var viewModel = provider.GetViewModel<TViewModel>(parameters);
    //        var container = provider.GetService<IIocConfiguration>();
    //        var view = container?.GetModalFor<TViewModel, TResult>(parameters) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
    //        view.Context = viewModel;
    //        return view.ShowDialogAsync();
    //    }
    //}
}
