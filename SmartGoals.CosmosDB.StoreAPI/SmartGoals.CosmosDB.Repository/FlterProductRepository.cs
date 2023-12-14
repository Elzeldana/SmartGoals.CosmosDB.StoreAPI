using Microsoft.Azure.Cosmos;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.StoreServices;

namespace SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Repository
{
    public class FlterProductRepository
    {
        private readonly Container _container;
        private readonly ICosmosDbClientService _cosmosDbClientService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FlterProductRepository> _logger;
        public FlterProductRepository(IConfiguration configuration,
            ICosmosDbClientService cosmosDbClientService,
            ILogger<FlterProductRepository> logger)
        {
            _configuration = configuration;
            _cosmosDbClientService = cosmosDbClientService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            string containerId = configuration["ProductContainer"]!;

            _container = cosmosDbClientService.GetContainer(containerId);

        }


    }
}
