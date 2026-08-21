using Avae.Core;
using BlazorNative.Core;
using Microsoft.Maui.Storage;

namespace Avae.Blazor.Essentials;

internal class BlazorSecureStorage(BlazorNative.Device.ISecureStorage secureStorage) : ISecureStorage
{
    public async Task<string?> GetAsync(string key)
    {
        var secret = await secureStorage.GetAsync(key);
        return secret.Value;
    }

    public bool Remove(string key)
    {
        return SecureStorageStatus.Ok == AsyncHelper.RunSync(async () => await secureStorage.DeleteAsync(key));
    }

    public void RemoveAll()
    {

    }

    public async Task SetAsync(string key, string value)
    {
        await secureStorage.SetAsync(key, value);
    }
}
