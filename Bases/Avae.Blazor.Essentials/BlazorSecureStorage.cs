using Avae.Core;
using BlazorNative.Core;
using Microsoft.Maui.Storage;

namespace Avae.Blazor.Essentials;

internal class BlazorSecureStorage : ISecureStorage
{
    public async Task<string?> GetAsync(string key)
    {
        var secureStorage = ServiceLocator.GetScopedRequiredService<BlazorNative.Device.ISecureStorage>();
        var secret = await secureStorage.GetAsync(key);
        return secret.Value;
    }

    public bool Remove(string key)
    {
        var secureStorage = ServiceLocator.GetScopedRequiredService<BlazorNative.Device.ISecureStorage>();
        return SecureStorageStatus.Ok == AsyncHelper.RunSync(async () => await secureStorage.DeleteAsync(key));
    }

    public void RemoveAll()
    {

    }

    public async Task SetAsync(string key, string value)
    {
        var secureStorage = ServiceLocator.GetScopedRequiredService<BlazorNative.Device.ISecureStorage>();
        await secureStorage.SetAsync(key, value);
    }
}
