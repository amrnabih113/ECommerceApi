using ECommerce.Models;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    string HashToken(string token);
}
