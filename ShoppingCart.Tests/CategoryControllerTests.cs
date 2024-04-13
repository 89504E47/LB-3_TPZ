using Moq;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Tests.Datasets;
using ShoppingCart.Web.Areas.Admin.Controllers;
using System.Linq.Expressions;
using Xunit;

namespace ShoppingCart.Tests
{
    public class CategoryControllerTests
    {
        [Fact]
        public void GetCategories_All_ReturnAllCategories()
        {
            // Arrange
            Mock<ICategoryRepository> repositoryMock = new Mock<ICategoryRepository>();

            repositoryMock.Setup(r => r.GetAll(It.IsAny<string>()))
                .Returns(() => CategoryDataset.Categories);
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.Equal(CategoryDataset.Categories, result.Categories);
        }
    }
}