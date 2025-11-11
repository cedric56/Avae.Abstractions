using Avae.Abstractions;
using Avae.Services;
using Avalonia.Threading;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using TaskDialogStandardResult = Avae.Abstractions.TaskDialogStandardResult;

namespace Avae.Implementations
{
    public class TaskDialogService : ITaskDialogService
    {
        public async Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
        {
            if (@params != null)
            {
                return await Dispatcher.UIThread.Invoke(async () =>
                {
                    try
                    {
                        TypedEventHandler<TaskDialog, EventArgs> opening = null;
                        TypedEventHandler<TaskDialog, EventArgs> opened = null;
                        TypedEventHandler<TaskDialog, TaskDialogClosingEventArgs> closing = null;
                        TypedEventHandler<TaskDialog, EventArgs> closed = null;

                        var taskDialog = new TaskDialog()
                        {
                            Buttons = CreateDialogButtons(results),
                            Content = @params.Content,
                            Title = @params.Title,
                            Header = @params.Header,
                            SubHeader = @params.SubHeader,
                            IconSource = (IconSource)@params.IconSource,
                            ShowProgressBar = @params.ShowProgressBar,
                            FooterVisibility = Enum.Parse<FluentAvalonia.UI.Controls.TaskDialogFooterVisibility>(@params.FooterVisibility.ToString()),
                            IsFooterExpanded = @params.IsFooterExpanded,
                            Footer = @params.Footer,
                            XamlRoot = TopLevelStateManager.GetActive()
                        };

                        taskDialog.Opening += opening = (sender, args) => @params.Opening?.Invoke();
                        taskDialog.Opened += opened = (sender, args) => @params.Opening?.Invoke();
                        taskDialog.Closing += closing = (sender, args) => args.Cancel = @params.Closing?.Invoke() ?? true;
                        taskDialog.Closed += closed = (sender, args) =>
                        {
                            @params.Closed?.Invoke();

                            taskDialog.Opened -= opened;
                            taskDialog.Opening -= opening;
                            taskDialog.Closing -= closing;
                            taskDialog.Closed -= closed;
                        };
                        var result = await taskDialog.ShowAsync();
                        return (TaskDialogStandardResult)result;
                    }
                    catch
                    {
                        return TaskDialogStandardResult.None;
                    }
                });
            }

            return TaskDialogStandardResult.None;
        }

        private static List<TaskDialogButton> CreateDialogButtons(TaskDialogStandardResult[] results)
        {
            var buttons = new List<TaskDialogButton>();
            foreach (var result in results)
            {
                TaskDialogButton? button = result switch
                {
                    TaskDialogStandardResult.OK => TaskDialogButton.OKButton,
                    TaskDialogStandardResult.Retry => TaskDialogButton.RetryButton,
                    TaskDialogStandardResult.Yes => TaskDialogButton.YesButton,
                    TaskDialogStandardResult.No => TaskDialogButton.NoButton,
                    TaskDialogStandardResult.Cancel => TaskDialogButton.CancelButton,
                    TaskDialogStandardResult.Close => TaskDialogButton.CloseButton,
                    _ => null
                };
                if (button != null)
                {
                    buttons.Add(button);
                }
            }
            return buttons;
        }
    }

}
