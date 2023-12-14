namespace SmartGoals.CosmosDB.StoreAPI.Requests
{
    public class UpdateProductRequest
    {
        public string? Name { get; set; }
        public string? CategoryId { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
    }

}
