using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using Xunit;
using Moq;
using ShoppingCart.Models;
using System.Linq.Expressions;


namespace ShoppingCart.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitofWork;

        
        public CategoryController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        [HttpGet]
        public CategoryVM Get()
        {
            CategoryVM categoryVM = new CategoryVM();
            categoryVM.Categories = _unitofWork.Category.GetAll(); // Удалите аргумент includeProperties
            return categoryVM;
        }


        [HttpGet]
        public CategoryVM Get(int id)
        {
            CategoryVM vm = new CategoryVM();

            vm.Category = _unitofWork.Category.GetT(x => x.Id == id);

            return vm;
        }

        [HttpPost]
        public void CreateUpdate(CategoryVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Category.Id == 0)
                {
                    _unitofWork.Category.Add(vm.Category);
                }
                else
                {
                    _unitofWork.Category.Update(vm.Category);
                }
                _unitofWork.Save();;
            }
            else
            {
                throw new Exception("Model is invalid");
            }
        }

        [HttpPost, ActionName("Delete")]
        public void DeleteData(int? id)
        {
            var category = _unitofWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }

            _unitofWork.Category.Delete(category);
            _unitofWork.Save();
        }
    }

    // NewTestClass
    public class CategoryControllerTests
    {

        //List
        public static IEnumerable<object[]> CategoryData =>
        new List<object[]>
        {
            new object[] { 0, "New Category" },  // Для создания новой категории
            new object[] { 1, "Existing Category" } // Для обновления существующей категории
        };

        [Theory]
        [MemberData(nameof(CategoryData))]

        //First Test
        public void CreateUpdate_AddsOrUpdateCategory(int categoryId, string categoryName)
        {
            // Arrange
            var category = new Category { Id = categoryId, Name = categoryName };
            var categoryVM = new CategoryVM { Category = category };

            var repositoryMock = new Mock<ICategoryRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Category).Returns(repositoryMock.Object);

            var controller = new CategoryController(unitOfWorkMock.Object);

            // Act
            controller.CreateUpdate(categoryVM);

            // Assert
            if (categoryId == 0)
            {
                repositoryMock.Verify(r => r.Add(category), Times.Once);
            }
            else
            {
                repositoryMock.Verify(r => r.Update(category), Times.Once);
            }

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        //Second Test
        [Fact]
        public void Get_ReturnsAllCategories()
        {
            // Arrange
            var expectedCategories = new List<Category> { new Category { Id = 1, Name = "Category 1" }, new Category { Id = 2, Name = "Category 2" } };
            var repositoryMock = new Mock<ICategoryRepository>();
            repositoryMock.Setup(r => r.GetAll(null)).Returns(expectedCategories);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Category).Returns(repositoryMock.Object);

            var controller = new CategoryController(unitOfWorkMock.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.Equal(expectedCategories, result.Categories);
        }

        // Third test
        [Fact]
        public void Get_ReturnsCategoryWithGivenId()
        {
            // Arrange
            int categoryId = 1;
            var expectedCategory = new Category { Id = categoryId, Name = "Category 1" };

            var repositoryMock = new Mock<ICategoryRepository>();
            repositoryMock.Setup(r => r.GetT(It.IsAny<Expression<Func<Category, bool>>>(), null))
                          .Returns(expectedCategory);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Category).Returns(repositoryMock.Object);

            var controller = new CategoryController(unitOfWorkMock.Object);

            // Act
            var result = controller.Get(categoryId);

            // Assert
            Assert.Equal(expectedCategory, result.Category);
        }

    }
}
