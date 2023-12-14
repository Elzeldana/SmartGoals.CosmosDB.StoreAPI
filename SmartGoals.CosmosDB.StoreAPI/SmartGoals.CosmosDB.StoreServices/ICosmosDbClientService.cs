using Microsoft.Azure.Cosmos;

namespace SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.StoreServices
{
    public interface ICosmosDbClientService
    {
        Container GetContainer(string containerId);
    }
}
