using SmartGoals.CosmosDB.StoreAPI.Models;
using SmartGoals.CosmosDB.StoreAPI.Requests;
using SmartGoals.CosmosDB.StoreAPI.Responses;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Controller;

namespace SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Repository
{
    public interface IProductRepository
    {
        Task<Product> GetItemAsync(string id, string partitionId);
        Task<List<Product>> GetAllItemsAsync();
        Task<Product> UpdateProductAsync(string id, string partitionKey, UpdateProductRequest productRequest);
        Task DeleteProductAsync(string id, string partitionKey);
        Task<List<ProductByPriceResponse>> FilterProductByPriceRange(int lower, int upper, int pageSize, int pageNum);
        Task CreateProductAsync(CreateProductRequest request);
        Task ProcessBulkProducts();


    }
}
