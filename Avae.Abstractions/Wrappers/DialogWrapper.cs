using System.Text;

namespace Avae.Abstractions
{
    public class DialogWrapper
    {
        private static string ToFullBlownString(Exception e, int level = int.MaxValue)
        {
            var sb = new StringBuilder();
            var exception = e;
            var counter = 1;
            while (exception != null && counter <= level)
            {
                //var stackFrame = (new StackTrace(exception, true)).GetFrame(0);
                //var message = string.Format("At line {0} column {1} in {2}: {3} {4}{3}{5}  ",
                //   stackFrame.GetFileLineNumber(), stackFrame.GetFileColumnNumber(),
                //   stackFrame.GetMethod(), Environment.NewLine, stackFrame.GetFileName(),
                //   exception.Message);

                sb.AppendLine($"{counter}-> Level: {counter}");
                sb.AppendLine($"{counter}-> Message: {exception.Message}");
                sb.AppendLine($"{counter}-> Source: {exception.Source}");
                sb.AppendLine($"{counter}-> Target Site: {exception.TargetSite}");
                sb.AppendLine($"{counter}-> Stack Trace: {exception.StackTrace}");
                //sb.AppendLine($"{counter}-> Formatted: {message}");

                exception = exception.InnerException;
                counter++;
            }

            return sb.ToString();
        }

        public static IDialogService Instance => SimpleProvider.GetService<IDialogService>();

        public static async Task ShowErrorAsync(Exception ex, string title = "Error", Action? callback = null)
        {
            var message = ToFullBlownString(ex);
            await Instance.ShowError(message, title,
                buttonText: "Ok",
                afterHideCallback: () => callback?.Invoke());
        }
        public static async Task ShowOkAsync(string message, string title = "Title")
        {
            await Instance.ShowError(message, title,
                buttonText: "Ok", () => { });
        }

        public static async Task<bool> ShowYesNoAsync(string message, string title = "Title")
        {
            return await Instance.ShowMessage(message, title, "YesNo",
                (o) => { }) == 0;
        }

        public static async Task<bool> ShowOkCancelAsync(string message, string title = "Title")
        {
            return await Instance.ShowMessage(message, title,
                "OkCancel",
                (o) => { }) == 0;
        }

        public static async Task<bool> ShowOkAbortAsync(string message, string title = "Title")
        {
            return await Instance.ShowMessage(message, title,
                "OkAbort",
                (o) => { }) == 0;
        }

        public static Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
        {
            return Instance.ShowMessage(message, title, "YesNoCancel", (o) => { });
        }

        public static Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
        {
            return Instance.ShowMessage(message, title, "YesNoAbort", (o) => { });
        }
    }
}
