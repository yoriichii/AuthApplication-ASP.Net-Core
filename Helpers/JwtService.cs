    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    namespace AuthApplication.Helpers
    {
        public class JwtService
        {
            private string secureKey = "MyVerySecureKeyThatIsLongEnough1234";

        // Method to generate the JWT token
        public string Generate(int id)
            {
            // Convert secure key to byte array 
            var sysmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));

            // Create signing credentials
            var credentials = new SigningCredentials(sysmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            // Create the JWT header with signing credentials
            var header = new JwtHeader(credentials);

            // Create the JWT payload (include user ID as issuer, set expiration to current time + 5 hours)
            var payload = new JwtPayload(
                issuer: id.ToString(), // User ID as issuer
                audience: null,        // Audience can be set if required
                claims: null,          // You can add custom claims here if needed
                notBefore: null,       // Optional: token is valid immediately
                expires: DateTime.UtcNow.AddHours(5) // Expiry set to 5 hours from current time
            );
            // Create the JWT security token
            var securityToken = new JwtSecurityToken(header, payload);

            // Return the serialized token as a string
            return new JwtSecurityTokenHandler().WriteToken(securityToken);

            }

        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secureKey);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }
    }
    }
