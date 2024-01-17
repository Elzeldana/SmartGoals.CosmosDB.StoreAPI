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

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List od products</returns>
        [HttpGet]
        public async Task<ActionResult> GetAllPoductItems()
        {
            var itemList = await _productRepository.GetAllItemsAsync();

            return Ok(itemList);
        }
        /// <summary>
        /// Create product
        /// </summary>
        /// <param name="createProductRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateProduct(CreateProductRequest createProductRequest)
        {
           await _productRepository.CreateProductAsync(createProductRequest);
            return Ok();

        }
        /// <summary>
        /// Get product by Id and Category Id 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="partitionKey">Category Id</param>
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
       /// Update Product Document
       /// </summary>
       /// <param name="id"> Product Record id </param>
       /// <param name="partitionKey">Category Id</param>
       /// <param name="product">Product request</param>
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
        /// <summary>
        /// Delete Procuct Document
        /// </summary>
        /// <param name="id">Document Id</param>
        /// <param name="partitionKey">Category Id</param>
        /// <returns></returns>
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
        /// <summary>
        /// Filter paginated Product list by Price 
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        [HttpGet("filterByPrice")]
        public async Task<IActionResult> FilterByPriceRange(int lower, int upper, int pageSize, int pageNum)
        {
            var items = await _productRepository.FilterProductByPriceRange(lower, upper, pageSize, pageNum);
            return Ok(items);
        }
        /// <summary>
        /// Insert Product Documents in Bulk
        /// </summary>
        /// <returns></returns>
        [HttpPost("insertBulk")]
        public async Task<IActionResult> InsertBulk()
        {
            await _productRepository.ProcessBulkProducts();
            return Ok();
        }
    }

}
