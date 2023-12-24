using Bogus;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using SmartGoals.CosmosDB.StoreAPI.Models;
using SmartGoals.CosmosDB.StoreAPI.Requests;
using SmartGoals.CosmosDB.StoreAPI.Responses;
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

                /// putt limit  !
                var query = _container.GetItemQueryIterator<Product>(
                    requestOptions: new QueryRequestOptions
                    {
                        MaxItemCount = 10, // Set MaxItemCount to -1 for unlimited results per page
                                           //  MaxConcurrency = -1, // Set MaxConcurrency to -1 for unlimited parallelism
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

        public async Task CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                Product product = new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = request.CategoryId,
                    CategoryName = request.CategoryName,
                    Description = request.Description,
                    Name = request.Name,
                    Price = request.Price,
                    Sku = request.Sku,
                    Tags = request.Tags,

                };

                await _container.CreateItemAsync(product, new PartitionKey(request.CategoryId));
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _logger.LogError($"Cosmos DB BadRequest: {cosmosEx.Message}");
                throw;
                // Optionally handle the exception, e.g., return an error response to the caller
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError($"Cosmos DB NotFound: {cosmosEx.Message}");
                throw;
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogError($"Cosmos DB Conflict: {cosmosEx.Message}");
                throw;

            }
            catch (CosmosException cosmosEx)
            {
                _logger.LogError($"Cosmos DB operation failed: {cosmosEx.Message}");
                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create product: {ex.Message}");
                throw;

            }


        }

        public async Task<Product> UpdateProductAsync(string id, string partitionKey, UpdateProductRequest productRequest)
        {
            try
            {
                ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, new PartitionKey(partitionKey));
                Product product = response.Resource;
                product.Description = productRequest.Description;
                product.Name = productRequest.Name;
                product.Tags = productRequest.Tags;
                product.Sku = productRequest.Sku;
                product.Price = productRequest.Price;
                //  await _container.ReplaceItemAsync<Product>(product, product.Id.ToString(), new PartitionKey(partitionKey));

                await _container.UpsertItemAsync(product);



                // check to delete partition key. 

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
        /// <param name = "id" ></ param >
        /// < param name="partitionKey"></param>
        /// <returns></returns>
        public async Task DeleteProductAsync(string id, string partitionKey)
        {
            try
            {
                ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, new PartitionKey(partitionKey));
                var product = response.Resource;

                await _container.DeleteItemAsync<Product>(product.Id.ToString(), new PartitionKey(partitionKey));
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

        public async Task<List<ProductByPriceResponse>> FilterProductByPriceRange(int lower, int upper, int pageSize, int pageNum)
        {
            try
            {
                QueryRequestOptions options = new QueryRequestOptions();
                options.MaxItemCount = pageSize;
                options.MaxBufferedItemCount = pageSize;

                string sql = "SELECT p.name  FROM products p  WHERE p.price >= @lower AND p.price <= @upper";
                QueryDefinition query = new QueryDefinition(sql)
                     .WithParameter("@lower", lower)
                     .WithParameter("@upper", upper);

                List<ProductByPriceResponse> productList = new List<ProductByPriceResponse>();
                FeedIterator<Product> iterator = _container.GetItemQueryIterator<Product>(query, requestOptions: options);

                int count = 0;
                while (iterator.HasMoreResults && count < (pageNum - 1) * pageSize)
                {
                    FeedResponse<Product> products = await iterator.ReadNextAsync();
                    count += products.Count;
                }

                while (iterator.HasMoreResults && productList.Count < pageSize)
                {
                    FeedResponse<Product> products = await iterator.ReadNextAsync();
                    foreach (var product in products)
                    {
                        ProductByPriceResponse res = new ProductByPriceResponse
                        {
                            CategoryId = product.CategoryId,
                            CategoryName = product.CategoryName,
                            Price = product.Price,
                            Description = product.Description,
                            Name = product.Name,
                            Sku = product.Sku,
                            Tags = product.Tags,
                            Id = product.Id,
                        };
                        productList.Add(res);
                    }
                }
                return productList;


            }


            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Cosmos DB error occurred: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                throw;
            }
        }

        public async Task ProcessBulkProducts()
        {
            try
            {
                CosmosClientOptions options = new()
                {
                    AllowBulkExecution = true
                };


                List<Product> productsToInsert = new Faker<Product>()
                    .StrictMode(true)
                    .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.CategoryId, f => f.Random.AlphaNumeric(5))
                .RuleFor(p => p.CategoryName, f => f.Commerce.Categories(1)[0])
                .RuleFor(p => p.Sku, f => f.Commerce.Ean13())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.Price, f => f.Random.Double(10, 1000))
                .RuleFor(p => p.Tags, f => new List<Tag>
                {
                new Tag { Id = f.Random.AlphaNumeric(5), Name = f.Commerce.Department(1) },
                new Tag { Id = f.Random.AlphaNumeric(5), Name = f.Commerce.Department(1) }
                }).Generate(100);

                List<Task> concurrentTasks = new List<Task>();

                foreach (Product product in productsToInsert)
                {
                    _logger.LogInformation($"{product.Id}, {product.Name}");
                    concurrentTasks.Add(
                        _container.CreateItemAsync(product, new PartitionKey(product.CategoryId))
                            .ContinueWith(task =>
                            {
                                if (task.IsFaulted && task.Exception != null)
                                {
                                    foreach (var ex in task.Exception.InnerExceptions)
                                    {
                                        _logger.LogError(ex, $"Cosmos DB Insertion Error {product.Id}");
                                    }
                                }
                            })
                    );
                }

                await Task.WhenAll(concurrentTasks);

                _logger.LogInformation("Bulk tasks complete");
            }

            catch (CosmosException cosmosException)
            {
                _logger.LogError(cosmosException, $"Cosmos DB Exception: Status code {cosmosException.StatusCode} Message: {cosmosException.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"General Exception: {ex.Message}");
            }
        }
    }
}

 










