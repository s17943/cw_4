using Cwiczenia_4Warehouse.Models;

namespace Cwiczenia_4Warehouse.Services;

public interface IDbService
{
    public interface IWarehouseDbService
    {
        public Task<int> AddProductToWarehouseAsync (Warehouse warehouse);
    }

    Task<int> AddProduct(Warehouse warehouse);
}