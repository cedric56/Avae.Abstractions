// ****************************************************************************
// <copyright file="IDialogService.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2016</copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>02.10.2014</date>
// <project>GalaSoft.MvvmLight</project>
// <web>http://www.mvvmlight.net</web>
// <license>
// See license.txt in this solution or http://www.galasoft.ch/license_MIT.txt
// </license>
// ****************************************************************************

namespace Avae.Abstractions;

/// <summary>
/// An interface defining how dialogs should be displayed in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IDialogService
{
    //
    // Summary:
    //     Displays information about an error.
    //
    // Parameters:
    //   message:
    //     The message to be shown to the user.
    //
    //   title:
    //     The title of the dialog box. This may be null.
    //
    //   buttonText:
    //     The text shown in the only button in the dialog box. If left null, the text "OK"
    //     will be used.
    //
    //   afterHideCallback:
    //     A callback that should be executed after the dialog box is closed by the user.
    //
    // Returns:
    //     A Task allowing this async method to be awaited.
    Task ShowError(string message, string title, string buttonText, Action afterHideCallback);
    //
    // Summary:
    //     Displays information about an error.
    //
    // Parameters:
    //   error:
    //     The exception of which the message must be shown to the user.
    //
    //   title:
    //     The title of the dialog box. This may be null.
    //
    //   buttonText:
    //     The text shown in the only button in the dialog box. If left null, the text "OK"
    //     will be used.
    //
    //   afterHideCallback:
    //     A callback that should be executed after the dialog box is closed by the user.
    //
    // Returns:
    //     A Task allowing this async method to be awaited.
    Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback);
    //
    // Summary:
    //     Displays information to the user. The dialog box will have only one button with
    //     the text "OK".
    //
    // Parameters:
    //   message:
    //     The message to be shown to the user.
    //
    //   title:
    //     The title of the dialog box. This may be null.
    //
    // Returns:
    //     A Task allowing this async method to be awaited.
    Task ShowMessage(string message, string title);
    //
    // Summary:
    //     Displays information to the user. The dialog box will have only one button.
    //
    // Parameters:
    //   message:
    //     The message to be shown to the user.
    //
    //   title:
    //     The title of the dialog box. This may be null.
    //
    //   buttonText:
    //     The text shown in the only button in the dialog box. If left null, the text "OK"
    //     will be used.
    //
    //   afterHideCallback:
    //     A callback that should be executed after the dialog box is closed by the user.
    //
    // Returns:
    //     A Task allowing this async method to be awaited.
    Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback);
    //
    // Summary:
    //     Displays information to the user. The dialog box will have only one button.
    //
    // Parameters:
    //   message:
    //     The message to be shown to the user.
    //
    //   title:
    //     The title of the dialog box. This may be null.
    //
    //   buttonConfirmText:
    //     The text shown in the "confirm" button in the dialog box. If left null, the text
    //     "OK" will be used.
    //
    //   buttonCancelText:
    //     The text shown in the "cancel" button in the dialog box. If left null, the text
    //     "Cancel" will be used.
    //
    //   afterHideCallback:
    //     A callback that should be executed after the dialog box is closed by the user.
    //     The callback method will get a boolean parameter indicating if the "confirm"
    //     button (true) or the "cancel" button (false) was pressed by the user.
    //
    // Returns:
    //     A Task allowing this async method to be awaited. The task will return true or
    //     false depending on the dialog result.
    Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool>? afterHideCallback = null);

    Task<int> ShowMessage(string message, string title, string buttonText, Action<int>? callback = null);
}

