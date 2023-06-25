using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using MyApp.UI.Common;

namespace MyApp.UI.Server.Data;

public class CurrentUserAccessor : IAccessorAsync<CurrentUser>
{
    private readonly AuthenticationStateProvider _auth;

    public CurrentUserAccessor(AuthenticationStateProvider auth)
    {
        _auth = auth;
    }

    public async Task<CurrentUser> Get()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        var userID = state.User.GetUserID();
        if (userID.IsNullOrEmpty())
            return null;

        return new CurrentUser(userID);
    }
}
