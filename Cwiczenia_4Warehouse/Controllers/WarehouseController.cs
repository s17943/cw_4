using Cwiczenia_4Warehouse.Models;
using Cwiczenia_4Warehouse.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia_4Warehouse.Controllers
{
    [Route("api/warehouse")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IDbService DbService;

        public WarehouseController(IDbService DbService)
        {
            this.DbService = DbService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody]Warehouse warehouse)
        {
            int id;
            try { id = await DbService.AddProduct(warehouse); }
            catch (Exception e) { return NotFound(e.Message); }
            
            return Ok($"Produkt o ID: {id} został dodany do Magazynu");
        }
    }
}