using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Repository;

namespace SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterProductController : ControllerBase
    {
        private readonly IFlterProductRepository _productRepository;
        public FilterProductController(IFlterProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


    }
}
