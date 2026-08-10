using Avalonia.Controls.Maui.Essentials;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

/// <summary>
/// Provides extension methods that bridge Avalonia UI's file sharing capabilities with 
/// the .NET MAUI Essentials Share API, enabling native platform sharing dialogs.
/// </summary>
/// <remarks>
/// This class converts Avalonia-specific file representations (AvaloniaFileResult) to MAUI-compatible 
/// ShareFile objects, allowing seamless file sharing across different platform implementations.
/// </remarks>
public static class AvaeShareExtensions
{
    /// <summary>
    /// Asynchronously shares multiple files using the native platform sharing dialog.
    /// </summary>
    /// <param name="share">The IShare instance used to invoke the native sharing UI.</param>
    /// <param name="title">The title or subject to display in the sharing dialog (platform-dependent).</param>
    /// <param name="files">A collection of files to share. Supports both standard MAUI files and AvaloniaFileResult types.</param>
    /// <returns>A Task representing the asynchronous share operation.</returns>
    /// <remarks>
    /// This method automatically detects and handles AvaloniaFileResult instances by wrapping them
    /// in AvaloniaShareFile adapters. Standard FileBase objects are used directly.
    /// 
    /// Note: On some platforms, this method may throw a PlatformNotSupportedException if sharing
    /// is not available, or a PermissionException if the user denies the share operation.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if share or files parameters are null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the files collection is empty.</exception>
    public static Task RequestAsync(this IShare share, string title, IEnumerable<FileBase> files)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentNullException.ThrowIfNull(files, nameof(files));

        // Convert the enumerable to a list to avoid multiple enumeration and get accurate count
        var shareFiles = new List<ShareFile>(files.Count());

        foreach (var file in files)
        {
            // Check if the file is an Avalonia-specific file result
            if (file is AvaloniaFileResult f)
                // Wrap it with the Avalonia adapter for proper handling
                shareFiles.Add(new AvaeShareFile(f));
            else
                // Use standard MAUI ShareFile for regular files
                shareFiles.Add(new ShareFile(file));
        }

        // Execute the native share request with the converted files
        return share.RequestAsync(new ShareMultipleFilesRequest()
        {
            Title = title,
            Files = shareFiles
        });
    }
}