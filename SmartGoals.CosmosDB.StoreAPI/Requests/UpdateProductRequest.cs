using SmartGoals.CosmosDB.StoreAPI.Models;

namespace SmartGoals.CosmosDB.StoreAPI.Requests
{
   
    public class UpdateProductRequest
    {
              
         //public string? CategoryId { get; set; }
         public string? CategoryName { get; set; }
         public string? Sku { get; set; }
         public string? Name { get; set; }
         public string? Description { get; set; }
         public double? Price { get; set; }
         public List<Tag>? Tags { get; set; }
    }
        
    }


