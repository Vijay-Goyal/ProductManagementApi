using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        [Authorize]
        [HttpGet]
        public List<Product> Get()
        {
            return _repository.GetAll();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _repository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _repository.Add(product);

            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Product product)
        {
            var existingProduct = _repository.GetById(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price;

            _repository.Update(existingProduct);

            return Ok(existingProduct);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _repository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            _repository.Delete(product);

            return Ok(product);
        }
    }
}