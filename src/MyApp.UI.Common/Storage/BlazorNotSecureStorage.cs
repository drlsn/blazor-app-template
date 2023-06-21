using Blazored.LocalStorage;
using Corelibs.Basic.Storage;

namespace MyApp.UI.Common.Storage;

public class BlazorNotSecureStorage : ISecureStorage
{
    private readonly ILocalStorageService _storage;

    public BlazorNotSecureStorage()
    {

    }

    public async Task<string> GetAsync(string key)
    {
        return null;
    }

    public bool Remove(string key)
    {
        return false;
    }

    public void RemoveAll()
    {
        
    }

    public async Task SetAsync(string key, string value)
    {
    }
}
