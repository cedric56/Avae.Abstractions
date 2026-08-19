using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

namespace Avae.Blazor.Essentials;

internal class BlazorLauncher(IJSRuntime jSRuntime) : ILauncher
{

    //Todo include BrowserEssentials.js
    private IJSObjectReference? _module;

    static readonly string[] NavigationSchemes = ["mailto", "tel", "sms"];

    public Task<bool> CanOpenAsync(Uri uri) =>
         Task.FromResult(uri.Scheme is "http" or "https" || NavigationSchemes.Contains(uri.Scheme));

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        return _module ??= await jSRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./js/appLauncher.js");
    }

    public async Task<bool> OpenAsync(Uri uri)
    {
        var module = await GetModuleAsync();
        return NavigationSchemes.Contains(uri.Scheme)
            ? await module.InvokeAsync<bool>("navigateTo", uri.AbsoluteUri)
            : await module.InvokeAsync<bool>("openUrl", uri.AbsoluteUri);
    }

    public async Task<bool> OpenAsync(OpenFileRequest request)
    {
        if (request.File is null)
            return false;

        var module = await GetModuleAsync();
        var bytes = await File.ReadAllBytesAsync(request.File.FullPath).ConfigureAwait(false);
        return await module.InvokeAsync<bool>("openFileBlob",
             Convert.ToBase64String(bytes),
             request.File.ContentType,
             Path.GetFileName(request.File.FullPath));
    }

    public async Task<bool> TryOpenAsync(Uri uri)
    {
        return await CanOpenAsync(uri).ConfigureAwait(false) && await OpenAsync(uri).ConfigureAwait(false);
    }
}
