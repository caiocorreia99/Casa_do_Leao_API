using CDL.Api.Controllers.v1.Interface;
using CDL.Models.Configuration;
using CDL.Models.DataBase;
using CDL.Models.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CDL.Api.Controllers.v1.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfiguration configuration;

        public TokenService(IOptions<JwtConfiguration> configuration)
        {
            this.configuration = configuration.Value;

            // Validate if the Key is set on the token service configuration
            if (string.IsNullOrWhiteSpace(this.configuration.Key))
                throw new ArgumentNullException(nameof(this.configuration.Key), "The provided Key on JwtConfiguration is missing.");
        }

        public string CreateJwtToken(User user, DateTime? expiration = null)
        {
            // Validate if the user instance is null
            if (user == null)
                throw new ArgumentNullException(nameof(user), "It's not possible to generate a new JwtToken for a null user.");

            var role = user.Role?.Code;
            if (string.IsNullOrWhiteSpace(role) || !UserRoles.IsValid(role))
                role = user.Admin ? UserRoles.Admin : UserRoles.Simple;

            var authClaims = new List<Claim>
            {
                new Claim("uid", user.IdUser.ToString()),
                new Claim("name", user.Name),
                new Claim("login", user.Email),
                new Claim("date", DateTime.Now.Ticks.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            // Get the key as a symetric security object
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Key));

            // Create the new generated JwtToken Object
            var token = new JwtSecurityToken
            (
                issuer: this.configuration.Issuer,
                audience: this.configuration.Audience,
                expires: expiration ?? DateTime.Now.Add(TimeSpan.FromMinutes(configuration.DurationInMinutes)),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            // Return the JwtToken processsed
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
