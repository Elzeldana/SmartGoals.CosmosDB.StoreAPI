using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartGoals.CosmosDB.StoreAPI.Requests;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Repository;
using System.Runtime.CompilerServices;

namespace SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class Products : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public Products(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(CreateProductRequest createProductRequest)
        {
            _productRepository.CreateProductAsync(createProductRequest);
            return Ok();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the Item</param>
        /// <param name="partitionKey">Partition key value< for categoryId /param>
        /// <returns></returns>
        [HttpGet("{id}/{partitionKey}")]
        public async Task<ActionResult> GetProduct(string id, string partitionKey)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(partitionKey))
            {
                return BadRequest();
            }

            var item = await _productRepository.GetItemAsync(id, partitionKey);
            return Ok(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAllPoductItems()
        {
            var itemList = await _productRepository.GetAllItemsAsync();

            return Ok(itemList);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task<ActionResult> UpdateProduct(string id, string partitionKey, UpdateProductRequest product)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(partitionKey))
            {
                return BadRequest();
            }

            return Ok(await _productRepository.UpdateProductAsync(id, partitionKey, product));

        }
        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(string id, string partitionKey)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(partitionKey))
            {
                return BadRequest();
            }

            await _productRepository.DeleteProductAsync(id, partitionKey);
            return NoContent();
        }

        [HttpGet("filterByPrice")]
        public async Task<IActionResult> FilterByPriceRange(int lower, int upper, int pageSize, int pageNum)
        {
            var items = await _productRepository.FilterProductByPriceRange(lower, upper, pageSize, pageNum);
            return Ok(items);
        }

        [HttpPost("insertBulk")]
        public async Task<IActionResult> InsertBulk()
        {
            await _productRepository.ProcessBulkProducts();
            return Ok();
        }
    }

}
