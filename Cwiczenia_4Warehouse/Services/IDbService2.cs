using Cwiczenia_4Warehouse.Models;

namespace Cwiczenia_4Warehouse.Services;

public interface IDbService2
{
    Task<int> AddProductToWarehouseAsync(Warehouse warehouse);
 }