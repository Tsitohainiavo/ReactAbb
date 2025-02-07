using Microsoft.AspNetCore.Mvc;
using ReactAbb.Server.Models;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace ReactAbb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenceController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AgenceController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetAgences()
        {
            try
            {
                using (IDbConnection connection = new OracleConnection(
                    _config.GetConnectionString("OracleConnection")))
                {
                    var agences = await connection.QueryAsync<Agence>(
                        "SELECT * FROM AGENCES"); // Remplacez par votre table des agences

                    return Ok(agences);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}