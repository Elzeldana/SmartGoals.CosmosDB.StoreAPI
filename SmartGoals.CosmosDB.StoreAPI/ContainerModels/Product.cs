namespace SmartGoals.CosmosDB.StoreAPI.Models
{
     public class Product
        {
            public string? id { get; set; }
            public string? name { get; set; }
            public string? categoryId { get; set; }
            public string? description { get; set; }
            public List<string>? tags { get; set; }

        }
 }

   


