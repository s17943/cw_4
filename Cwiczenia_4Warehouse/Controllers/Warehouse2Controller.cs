using Cwiczenia_4Warehouse.Models;
using Cwiczenia_4Warehouse.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia_4Warehouse.Controllers;

[Route("api/warehouse2")]
[ApiController]
    public class Warehouse2Controller : ControllerBase
    {
        private readonly IDbService2 dbService2;

        public Warehouse2Controller(IDbService2 dbService2)
        {
            this.dbService2 = dbService2;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Warehouse warehouse)
        {
            int id;
            try { id = await dbService2.AddProductToWarehouseAsync(warehouse); }
            catch (Exception e) { return NotFound(e.Message); }
            return Ok($"Produkt o ID: {id} został dodany do Magazynu");
        }
    }
