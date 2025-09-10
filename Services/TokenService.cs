using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestRegistrationForWS.Services;

public class TokenService
{
    private readonly List<string> _inMemoryTokens = new();

    private readonly IConfiguration _configuration;

    // 1. Внедряем IConfiguration, чтобы получить доступ к appsettings.json
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool TokenExists(string token)
    {
        return _inMemoryTokens.Contains(token);
    }

    public string CreateToken(string wsName, string clientId, string cityId)
    {
        if (!CanCreateToken(wsName, clientId, cityId))
            throw new UnauthorizedAccessException("Cannot create token for this client");

        // 2. Создаем ключ безопасности из нашего секрета
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        // 3. Создаем "учетные данные" для подписи токена
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 4. Определяем "утверждения" (claims) - информацию внутри токена
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, clientId), // Идентификатор клиента
            new Claim("cityId", cityId),                      // Наше кастомное поле
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Уникальный ID токена
        };

        // 5. Создаем сам токен
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(45), // Срок жизни токена
            signingCredentials: credentials);

        // 6. Сериализуем токен в строку
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        //TODO: Додати збереження токена в редіс

        return tokenString;
    }

    public bool CanCreateToken(string wsName, string clientId, string cityId)
    {
        // Тут твоя логіка перевірки прав для створення токена
        return true; // тимчасово завжди дозволено
    }
}