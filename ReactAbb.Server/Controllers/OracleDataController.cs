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

    }
}