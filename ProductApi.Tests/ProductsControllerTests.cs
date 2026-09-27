using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.Controllers;
using ProductApi.Repositories;

namespace ProductApi.Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public void Get_ReturnsProducts()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();

            repository.Setup(r => r.GetAll())
                .Returns(new List<ProductApi.Models.Product>
                {
                    new ProductApi.Models.Product
                    {
                        Id = 1,
                        ProductName = "Test Laptop",
                        Price = 50000
                    }
                });

            var controller = new ProductsController(repository.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetById_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();

            repository.Setup(r => r.GetById(1))
                .Returns(new ProductApi.Models.Product
                {
                    Id = 1,
                    ProductName = "Test Laptop",
                    Price = 50000
                });

            var controller = new ProductsController(repository.Object);

            // Act
            var result = controller.GetById(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();

            repository.Setup(r => r.GetById(999))
                .Returns((ProductApi.Models.Product?)null);

            var controller = new ProductsController(repository.Object);

            // Act
            var result = controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_AddsProduct()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();

            var controller = new ProductsController(repository.Object);

            var product = new ProductApi.Models.Product
            {
                ProductName = "Test Mouse",
                Price = 1000
            };

            // Act
            var result = controller.Create(product);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            repository.Verify(
                r => r.Add(product),
                Times.Once);
        }

        [Fact]
        public void Update_UpdatesProduct_WhenProductExists()
        {
            // Arrange
            var existingProduct = new ProductApi.Models.Product
            {
                Id = 1,
                ProductName = "Old Laptop",
                Price = 40000
            };

            var repository = new Mock<IProductRepository>();

            repository.Setup(r => r.GetById(1))
                .Returns(existingProduct);

            var controller = new ProductsController(repository.Object);

            var updatedProduct = new ProductApi.Models.Product
            {
                ProductName = "New Laptop",
                Price = 50000
            };

            // Act
            var result = controller.Update(1, updatedProduct);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            Assert.Equal("New Laptop", existingProduct.ProductName);
            Assert.Equal(50000, existingProduct.Price);

            repository.Verify(
                r => r.Update(existingProduct),
                Times.Once);
        }

        [Fact]
        public void Delete_RemovesProduct_WhenProductExists()
        {
            // Arrange
            var product = new ProductApi.Models.Product
            {
                Id = 1,
                ProductName = "Test Keyboard",
                Price = 2000
            };

            var repository = new Mock<IProductRepository>();

            repository.Setup(r => r.GetById(1))
                .Returns(product);

            var controller = new ProductsController(repository.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            repository.Verify(
                r => r.Delete(product),
                Times.Once);
        }

        [Fact]
        public void Update_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();

            repository.Setup(r => r.GetById(999))
                .Returns((ProductApi.Models.Product?)null);

            var controller = new ProductsController(repository.Object);

            var product = new ProductApi.Models.Product
            {
                ProductName = "New Product",
                Price = 1000
            };

            // Act
            var result = controller.Update(999, product);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            repository.Verify(
                r => r.GetById(999),
                Times.Once);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();

            repository.Setup(r => r.GetById(999))
                .Returns((ProductApi.Models.Product?)null);

            var controller = new ProductsController(repository.Object);

            // Act
            var result = controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            repository.Verify(
                r => r.GetById(999),
                Times.Once);
        }
    }
}