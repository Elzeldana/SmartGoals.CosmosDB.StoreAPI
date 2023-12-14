using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using SmartGoals.CosmosDB.StoreAPI.Models;
using SmartGoals.CosmosDB.StoreAPI.Requests;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Controller;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.StoreServices;

namespace SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly Container _container;
        private readonly ICosmosDbClientService _cosmosDbClientService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(IConfiguration configuration,
            ICosmosDbClientService cosmosDbClientService,
            ILogger<ProductRepository> logger)
        {
            _configuration = configuration;
            _cosmosDbClientService = cosmosDbClientService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            string containerId = configuration["ProductContainer"]!;

            _container = cosmosDbClientService.GetContainer(containerId);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="partitionId"></param>
        /// <param name="databaseId"></param>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public async Task<Product> GetItemAsync(string id, string partitionId)
        {
            try
            {
                PartitionKey key = new PartitionKey(partitionId);

                ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, key);
                return response.Resource;
            }
            catch (CosmosException cosmosEx)
            {
                _logger.LogError(cosmosEx, "Cosmos DB exception while getting item: {Message}", cosmosEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting item from Cosmos DB: {Message}", ex.Message);
                throw;
            }
        }

        //public async Task<List<Product>> GetAllItemsAsync()
        //{
        //    List<Product> allProducts = new List<Product>();

        //    try
        //    {
        //        // better use the query Iterator than linq
        //        var query = _container.GetItemQueryIterator<Product>(
        //            new QueryDefinition("SELECT * FROM c"));

        //        while (query.HasMoreResults)
        //        {
        //            FeedResponse<Product> response = await query.ReadNextAsync();
        //            allProducts.AddRange(response.ToList());
        //        }

        //        return allProducts;
        //    }
        //    catch (CosmosException cosmosEx)
        //    {
        //        _logger.LogError(cosmosEx, "Cosmos DB exception while getting all items: {Message}", cosmosEx.Message);
        //        // Handle Cosmos DB exception here or rethrow if required
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while getting all items from Cosmos DB: {Message}", ex.Message);
        //        throw;
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetAllItemsAsync()
        {
            List<Product> allProducts = new List<Product>();

            try
            {
                var query = _container.GetItemQueryIterator<Product>(
                    requestOptions: new QueryRequestOptions
                    {
                        MaxItemCount = -1, // Set MaxItemCount to -1 for unlimited results per page
                        MaxConcurrency = -1, // Set MaxConcurrency to -1 for unlimited parallelism
                    });

                while (query.HasMoreResults)
                {
                    FeedResponse<Product> response = await query.ReadNextAsync();
                    allProducts.AddRange(response);
                }

                return allProducts;
            }
            catch (CosmosException cosmosEx)
            {
                _logger.LogError(cosmosEx, "Cosmos DB exception while getting all items: {Message}", cosmosEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting all items from Cosmos DB: {Message}", ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productRequest"></param>
        /// <returns></returns>
        public async Task<Product> UpdateProductAsync(string id, string partitionKey, UpdateProductRequest productRequest)
        {
            try
            {
                ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, new PartitionKey(partitionKey));
                Product product = response.Resource;
                product.description = productRequest.Description;
                product.name = productRequest.Name;
                product.tags = productRequest.Tags;

                await _container.ReplaceItemAsync(product, id, new PartitionKey(partitionKey));

                return product;
            }

            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError(cosmosEx, "Product with ID {Id} not found.", id);
                throw;
            }
            catch (CosmosException cosmosEx)
            {
                _logger.LogError(cosmosEx, "Cosmos DB exception while updating product with ID {Id}.", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID {Id}.", id);
                throw;
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task DeleteProductAsync ( string id, string partitionKey)
        {
            try
            {
               ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, new PartitionKey(partitionKey));
                var product = response.Resource;

                await _container.DeleteItemAsync<Product>(product.id, new PartitionKey(partitionKey));
                _logger.LogInformation($"Product with ID: {id} deleted successfully.");
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Product with ID: {id} not found.");
                throw; 
            }
            catch (CosmosException cosmosEx)
            {
                _logger.LogError(cosmosEx, $"Cosmos DB exception while deleting product with ID: {id}.");
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting product with ID: {id}.");
                throw; 
            }

        }
    }
}






