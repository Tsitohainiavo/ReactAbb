using Dapper;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ReactAbb.Server.Models;
using System.Data;

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

                    // Retournez une réponse réussie (vous pouvez également retourner un token JWT ici)
                    return Ok(new { Message = "Connexion réussie", Utilisateur = utilisateur });
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