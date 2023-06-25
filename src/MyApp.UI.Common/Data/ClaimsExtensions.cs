using System.Security.Claims;

namespace MyApp.UI.Common
{
    public static class ClaimsExtensions
    {
        public static string GetUserID(this ClaimsPrincipal claims)
        {
            var idClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                return null;

            return idClaim.Value;
        }
    }
}
