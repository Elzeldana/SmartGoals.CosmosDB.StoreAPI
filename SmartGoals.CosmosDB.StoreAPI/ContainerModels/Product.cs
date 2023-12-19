namespace SmartGoals.CosmosDB.StoreAPI.Models
{
     
        public class Tag
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class Product
        {
            public string Id { get; set; }
            public string CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string Sku { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Price { get; set; }
            public List<Tag> Tags { get; set; }
        }

    }
 

   


