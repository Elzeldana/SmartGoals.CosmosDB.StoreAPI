using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.StoreServices
{
    public class CosmosDbClientService : ICosmosDbClientService
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, CosmosClient> _cosmosClients = new();
        private readonly Dictionary<string, Container> _cosmosContainers = new();

        public CosmosDbClientService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Container GetContainer(/*string databaseId*/ string containerId)
        {
            var databaseId = _configuration["DatabaseId"]!;
            string containerKey = $"{databaseId}-{containerId}";

            if (!_cosmosContainers.ContainsKey(containerKey))
            {
                CosmosClient cosmosClient = GetOrCreateCosmosClient(databaseId);
                Database database = cosmosClient.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId);
                _cosmosContainers.Add(containerKey, container);
            }

            return _cosmosContainers[containerKey];
        }

        private CosmosClient GetOrCreateCosmosClient(string databaseId)
        {
            if (!_cosmosClients.ContainsKey(databaseId))
            {
                string? connectionString = _configuration["CosmosDBConnectionString"];
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("CosmosDBConnectionString is missing or empty in configuration.");
                }

                _cosmosClients[databaseId] = new CosmosClient(connectionString);
            }

            return _cosmosClients[databaseId];
        }
    }
}
