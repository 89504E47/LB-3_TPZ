using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;

namespace ShoppingCart.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        IWebHostEnvironment _hostingEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public ProductVM Get(int? id)
        {
            ProductVM vm = new ()
            {
                Product = new(),
                Categories = _unitOfWork.Category.GetAll().Select(x =>
                new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                return vm;
            }
            else
            {
                vm.Product = _unitOfWork.Product.GetT(x => x.Id == id);
                if (vm.Product == null)
                {
                    throw new Exception("Product not found");
                }
                else
                {
                    return vm;
                }
            }
        }

        [HttpPost]
        public void CreateUpdate(ProductVM vm)
        {
            if(ModelState.IsValid)
            {
                if (vm.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(vm.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(vm.Product);
                }
                _unitOfWork.Save();
            }
            else
            {
                throw new Exception("Model invalid");
            }
        }

        [HttpDelete]
        public IActionResult Delete (int? id)
        {
            var product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error in Fetching Data" });
            }
            else
            {
                _unitOfWork.Product.Delete(product);
                _unitOfWork.Save();

                return Json(new { success = true, message = "Product Deleted" });
            }
        }
    }
}
