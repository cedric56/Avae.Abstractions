using Avae.Core;
using BlazorNative.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

internal class BlazorSecureStorage(CircuitServiceAccessor circuitServiceAccessor) : ISecureStorage
{
    public async Task<string?> GetAsync(string key)
    {
        var secureStorage = circuitServiceAccessor.GetRequiredService<BlazorNative.Device.ISecureStorage>();
        var secret = await secureStorage.GetAsync(key);
        return secret.Value;
    }

    public bool Remove(string key)
    {
        var secureStorage = circuitServiceAccessor.GetRequiredService<BlazorNative.Device.ISecureStorage>();
        return SecureStorageStatus.Ok == AsyncHelper.RunSync(async () => await secureStorage.DeleteAsync(key));
    }

    public void RemoveAll()
    {

    }

    public async Task SetAsync(string key, string value)
    {
        var secureStorage = circuitServiceAccessor.GetRequiredService<BlazorNative.Device.ISecureStorage>();
        await secureStorage.SetAsync(key, value);
    }
}
