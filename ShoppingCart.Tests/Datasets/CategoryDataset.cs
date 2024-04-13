using ShoppingCart.Models;

namespace ShoppingCart.Tests.Datasets
{
    public static class CategoryDataset
    {
        public static List<Category> Categories = new List<Category>
        {
            new Category
            {
                DisplayOrder = 0,
                CreatedDateTime = DateTime.Now,
                Id = 1,
                Name = "Test1"
            },
            new Category
            {
                DisplayOrder = 1,
                CreatedDateTime = DateTime.Now,
                Id = 2,
                Name = "Test2"
            },
            new Category
            {
                DisplayOrder = 2,
                CreatedDateTime = DateTime.Now,
                Id = 3,
                Name = "Test3"
            },
            new Category
            {
                DisplayOrder = 3,
                CreatedDateTime = DateTime.Now,
                Id = 4,
                Name = "Test4"
            },
        };
    }
}
