using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020526.BusinessLayers;
using SV21T1020526.DomainModels;
using SV21T1020526.Web.Appcodes;
using SV21T1020526.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020526.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]

    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };

            return View(condition);
        }

        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "",
                    condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                Data = data,
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "thêm mặt hàng";
            var data = new Product()
            {
                ProductID = 0
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "sửa thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
            //kiểm tra dữ liệu đầu vào
            // ModelState.AddModelError(key, message)
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (data.CategoryID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Loại hàng không được để trống");
            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Nhà cung cấp không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị của mặt hàng");
            
            if (data.Price <= 0 )
            {
                ModelState.AddModelError(nameof(data.Price), "Giá phải là một số thập phân dương");
            }
            //dựa vào ModelState để biết có tồn tại lỗi? sử dụng ModelState.IsValid(true: không lỗi)
            if (!ModelState.IsValid)
            {
                return View("Edit", data); // trả dữ liệu về view và các lỗi
            }

            // xử lý ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
				string filePathShop = @"D:\DHNam\SV21T1020526\SV21T1020526.Shop\wwwroot\images\product";
				if (!Directory.Exists(filePathShop))
				{
					Directory.CreateDirectory(filePathShop); // Tạo thư mục nếu chưa tồn tại
				}
				string fullFilePathShop = Path.Combine(filePathShop, fileName);
				using (var stream = new FileStream(fullFilePathShop, FileMode.Create))
				{
					uploadPhoto.CopyTo(stream);
				}
				data.Photo = fileName;

            }


            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Tên sản phẩm bị trùng");
                    return View("Edit", data);
                }
                return RedirectToAction("Edit", new { id = id });
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Tên sản phẩm bị trùng");
                    return View("Edit", data);
                }

            }
            return RedirectToAction("Edit", data);

        }

        public IActionResult Delete(int id = 0)
        {

            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }

            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);

        }

        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    var data = new ProductAttribute()
                    {
                        ProductID = id,
                        AttributeID = attributeId,
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính cho mặt hàng";
                    var dataAttribute = ProductDataService.GetAttribute(attributeId);
                    return View(dataAttribute);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính cho mặt hàng" : "Cập nhật thuộc tính cho mặt hàng";
            //kiểm tra dữ liệu đầu vào
            // ModelState.AddModelError(key, message)
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng nhập thứ tự hiển thị ảnh trong thư viện");
            
            if (!ModelState.IsValid)
            {
                return View("Attribute", data); // trả dữ liệu về view và các lỗi
            }

            if(data.AttributeID <= 0)
            {
                ProductDataService.AddAttribute(data);
            }
            else
            {
                ProductDataService.UpdateAttribute(data);
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    var data = new ProductPhoto()
                    {
                        IsHidden = true,
                        ProductID = id,
                        PhotoID = 0
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Cập nhật thông tin ảnh của mặt hàng";
                    var dataEdit = ProductDataService.GetPhoto(photoId);
                    return View(dataEdit);
                case "delete":
                    ViewBag.Title = "xóa ảnh mặt hàng";
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id});
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {

            ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Cập nhật thông tin ảnh của mặt hàng";
            //kiểm tra dữ liệu đầu vào
            // ModelState.AddModelError(key, message)
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Tên mặt hàng không được để trống");
            if (data.DisplayOrder<=0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng thứ tự hiển thị ảnh trong thư viện");
            //dựa vào ModelState để biết có tồn tại lỗi? sử dụng ModelState.IsValid(true: không lỗi)
            if (uploadPhoto == null)
                ModelState.AddModelError(nameof(data.Photo), "vui lòng upload ảnh");

            if (!ModelState.IsValid)
            {
                return View("Photo", data); // trả dữ liệu về view và các lỗi
            }
            // xử lý ảnh
            string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
            string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products\productPhotos", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                uploadPhoto.CopyTo(stream);
            }
            
            string filePathPhoto = Path.Combine(ApplicationContext.WebRootPath, @"D:\DHNam\SV21T1020526\SV21T1020526.Shop\wwwroot\images\product\productPhoto", fileName);
            using (var stream=new FileStream(filePathPhoto, FileMode.Create))
            {
                uploadPhoto.CopyTo(stream);
            }
            data.Photo = fileName;
            if(data.PhotoID <= 0)
            {
                long id = ProductDataService.AddPhoto(data);
            }

            else
            {
                ProductDataService.UpdatePhoto(data);
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }

    }
}
