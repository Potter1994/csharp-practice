using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public AccessToken Create(User user)
    {
        // 產生 JWT

        string secretKey = configuration.GetValue<string>("Jwt:Key") ?? "";
        var issuer = configuration.GetValue<string>("Jwt:Issuer");
        var audience = configuration.GetValue<string>("Jwt:Audience");
        var expiryMinutes = configuration.GetValue<int>("Jwt:ExpiryMinutes", 5);

        // 設定 Claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        // 使用 Secret Key 簽章

        // 對稱式簽章金鑰(Symmetric Key)
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 設定 Expiration
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            SigningCredentials = credentials,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        // JwtSecurityToken 物件,還沒編碼
        var securityToken = tokenHandler.CreateToken(descriptor); // {"alg":"HS256","typ":"JWT"}.{"nbf":1793869926,"exp":1793869986, ...}

        // Encoded Token 編碼成 header.payload.signature(三段都是 base64url)
        var serializeToken = tokenHandler.WriteToken(securityToken);

        return new AccessToken(serializeToken, (DateTimeOffset)descriptor.Expires);
    }
}
