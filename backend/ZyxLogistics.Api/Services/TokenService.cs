using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime ExpiraEm) GenerateToken(AuthUsuario usuario, IReadOnlyList<Permissao> permissoes)
        {
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer nao configurado.");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience nao configurado.");
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key nao configurado.");
            var expiresMinutes = _configuration.GetValue<int?>("Jwt:ExpiresMinutes") ?? 480;
            var expiraEm = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nome),
                new(ClaimTypes.Email, usuario.Email),
                new("perfilId", usuario.PerfilId.ToString()),
                new("perfilDescricao", usuario.PerfilDescricao)
            };

            claims.AddRange(permissoes.Select(permissao => new Claim("permission", permissao.Codigo)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiraEm,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
        }
    }
}
