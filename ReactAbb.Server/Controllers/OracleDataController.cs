using Dapper;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ReactAbb.Server.Models;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ReactAbb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OracleDataController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OracleDataController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize]
        [HttpGet("utilisateurs")]
        public async Task<IActionResult> GetUtilisateurs()
        {
            try
            {
                using (IDbConnection connection = new OracleConnection(
                    _config.GetConnectionString("OracleConnection")))
                {
                    var utilisateurs = await connection.QueryAsync<Utilisateurs>(
                        "SELECT * FROM UTILISATEURS");

                    return Ok(utilisateurs);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                using (var connection = new OracleConnection(
                    _config.GetConnectionString("OracleConnection")))
                {
                    await connection.OpenAsync();
                    return Ok("Connection successful");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Connection failed: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                using (IDbConnection connection = new OracleConnection(
                    _config.GetConnectionString("OracleConnection")))
                {
                    var utilisateur = await connection.QueryFirstOrDefaultAsync<Utilisateurs>(
                        "SELECT * FROM UTILISATEURS WHERE EMAIL = :Email",
                        new { Email = loginRequest.Email });

                    if (utilisateur == null)
                    {
                        return Unauthorized("Utilisateur non trouvé.");
                    }

                    // Vérifiez le mot de passe haché
                    if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, utilisateur.PASSWORD))
                    {
                        return Unauthorized("Mot de passe incorrect.");
                    }

                    // Générer un token JWT
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]); // Récupérer la clé depuis appsettings.json
                    if (key.Length < 16)
                    {
                        throw new ArgumentException("La clé JWT doit avoir au moins 128 bits (16 caractères).");
                    }

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, utilisateur.EMAIL),
                            new Claim(ClaimTypes.NameIdentifier, utilisateur.ID.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        Issuer = _config["Jwt:Issuer"],
                        Audience = _config["Jwt:Audience"],
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    // Retournez le token JWT
                    return Ok(new { Token = tokenString, Message = "Connexion réussie", Utilisateur = utilisateur });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                using IDbConnection connection = new OracleConnection(
                    _config.GetConnectionString("OracleConnection"));
                // Vérifiez si l'utilisateur existe déjà
                var existingUser = await connection.QueryFirstOrDefaultAsync<Utilisateurs>(
                    "SELECT * FROM UTILISATEURS WHERE EMAIL = :Email",
                    new { Email = registerRequest.Email });

                if (existingUser != null)
                {
                    return BadRequest("Un utilisateur avec cet email existe déjà.");
                }

                // Hacher le mot de passe
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

                // Insérer le nouvel utilisateur dans la base de données
                var sql = @"
                INSERT INTO UTILISATEURS (EMAIL,PASSWORD)
                VALUES (:Email, :Password)";
                await connection.ExecuteAsync(sql, new
                {
                    Email = registerRequest.Email,
                    Password = hashedPassword
                });

                return Ok("Inscription réussie !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

    }
}