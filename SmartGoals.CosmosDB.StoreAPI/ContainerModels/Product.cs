using Newtonsoft.Json;

namespace SmartGoals.CosmosDB.StoreAPI.Models
{

    public class Tag
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Product
        {


        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("categoryId")]
        public string? CategoryId { get; set; }
        [JsonProperty("categoryName")]
        public string? CategoryName { get; set; }
        [JsonProperty("sku")]
        public string? Sku { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("price")]
        public double? Price { get; set; }
        [JsonProperty("tags")]
        public List<Tag>? Tags { get; set; }
    }

    }
 

   


