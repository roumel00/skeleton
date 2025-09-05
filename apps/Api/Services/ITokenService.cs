using Api.Entities;

namespace Api.Services;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    void SetAuthCookie(string token);
    void ClearAuthCookie();
}
