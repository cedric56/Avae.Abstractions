using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;

namespace Avae.Shared;

public static class Extensions
{
    public static Task ComposeAsync(this IEmail email, IEnumerable<FileBase> files, EmailMessage message)
    {
        if (email is IAvaeEmail avae)
        {
            return avae.ComposeAsync(files, message);
        }
        else
        {
            var attachments = new List<EmailAttachment>();
            foreach (var file in files ?? [])
            {
                attachments.Add(new EmailAttachment(file.FullPath));
            }
            message.Attachments = attachments;
            return email.ComposeAsync(message);
        }
    }

    public static Task RequestAsync(this IShare share, string title, IEnumerable<FileBase> files)
    {
        if (share is IAvaeShare avae)
        {
            return avae.RequestAsync(title, files);
        }
        else
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNull(files, nameof(files));

            // Convert the enumerable to a list to avoid multiple enumeration and get accurate count
            var shareFiles = new List<ShareFile>(files.Count());

            foreach (var file in files)
            {
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
}
