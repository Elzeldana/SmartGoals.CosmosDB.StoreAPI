using Microsoft.Azure.Cosmos;
using SmartGoals.CosmosDB.StoreAPI.Models;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Repository;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.StoreServices;


public class CosmosDbGenericRepository<T> : ICosmosDbGenericRepository<T> where T : class
{
    private readonly CosmosDbClientService _cosmosDbClientService;
    private readonly ILogger<CosmosDbGenericRepository<T>> _logger;

    public CosmosDbGenericRepository(CosmosDbClientService cosmosDbClientService, ILogger<CosmosDbGenericRepository<T>> logger) 
        
    {
        _cosmosDbClientService = cosmosDbClientService ?? throw new ArgumentNullException(nameof(cosmosDbClientService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    
  
}
