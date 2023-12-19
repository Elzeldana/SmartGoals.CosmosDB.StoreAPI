// See https://aka.ms/new-console-template for more information
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.ConsoleApp;

//CosmosClient client = 
//    new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
//Database database = await client.CreateDatabaseIfNotExistsAsync("elenadb");
//Container container = await database.CreateContainerIfNotExistsAsync("products", "/categoryId", 400);

string endpoint = "https://localhost:8081";

string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

CosmosClient client = new CosmosClient(endpoint, key);

Database database = await client.CreateDatabaseIfNotExistsAsync("SmartGoalsStoreDB");

Container container = await database.CreateContainerIfNotExistsAsync("Product", "/categoryName");

string sql = "SELECT * FROM c where c.categoryId = '26C74104-40BC-4541-8EF5-9892F7F03D72'";
QueryDefinition query = new(sql);

QueryRequestOptions options = new();
options.MaxItemCount = 2;

FeedIterator<Product> iterator = container.GetItemQueryIterator<Product>(query, requestOptions: options);

while (iterator.HasMoreResults)
{
    FeedResponse<Product> products = await iterator.ReadNextAsync();
    foreach (Product product in products)
    {
        Console.WriteLine($"[{product.id}]\t[{product.name,40}]\t[{product.price,10}]");
    }
    Console.WriteLine("Press any key for next page of results");
    Console.ReadKey();
}
////Product product = new()
////{
////    id = Guid.NewGuid().ToString(),
////    categoryId = Guid.NewGuid().ToString(),
////    name = "Some New Product",
////    description = "Description",

////};

////await container.CreateItemAsync(product);

//var id = "45328335-b445-45fa-80d5-db9ca82bd672";
//var partitionKey = "56931ddf-1a1b-42d4-abec-a557a20a2e1c";
//PartitionKey key = new(partitionKey);

//var product = await container.ReadItemAsync<Product>(id, key);

//Console.WriteLine(product.Resource);
//Console.ReadLine(); 