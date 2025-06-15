using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020526.BusinessLayers;
using SV21T1020526.DomainModels;
using SV21T1020526.Shop.Appcodes;
using SV21T1020526.Shop.Models;

namespace SV21T1020526.Shop.Controllers
{
	[Authorize(Roles = WebUserRoles.CUSTOMER)]
	public class ShoppingCartController : Controller
	{
		private const string SHOPPING_CART = "ShoppingCart";
		public IActionResult Index()
		{
			var shoppingCart = GetShoppingCart();
			return View(shoppingCart);
		}

		public IActionResult OrderStatus()
		{
			int customerId = int.Parse(User.GetUserData().UserId);
			var data =OrderDataService.GetOrderByCustomerID(customerId);
			return View(data);
		}
		
		public IActionResult Save(string customerContactName = "",string deliveryProvince = "", string deliveryAddress = "")
		{
			var shoppingCart = GetShoppingCart();
			if (shoppingCart.Count == 0)
			{
				return Json("Giỏ hàng trống, vui lòng chọn mặt hàng cần mua");
			}
			var userData = User.GetUserData();
			int customerID = int.Parse(userData.UserId);
			if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
			{
				return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
			}
			
			
			List<OrderDetail> orderDetails = new List<OrderDetail>();
			foreach (var item in shoppingCart)
			{
				orderDetails.Add(new OrderDetail()
				{
					ProductID = item.ProductID,
					Quantity = item.Quantity,
					SalePrice = item.SalePrice,
				});
			}
			int orderID = OrderDataService.Shopping( customerID, deliveryProvince, deliveryAddress,customerContactName, orderDetails);

			ClearCart();
			return Json(orderID);

		}
		private List<CartItem> GetShoppingCart()
		{
			var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
			if (shoppingCart == null)
			{
				shoppingCart = new List<CartItem>();
				ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
			}

			return shoppingCart;
		}

		[Authorize]
		[HttpPost]
		public IActionResult AddToCart(int productId, int quantity)
		{
			// Kiểm tra nếu quantity hợp lệ
			if (quantity <= 0)
			{
				return Json(new { Success = false, Message = "Số lượng không hợp lệ.",count=0 });
			}

			// Lấy giỏ hàng từ session
			var shoppingCart = GetShoppingCart();

			// Tìm sản phẩm đã có trong giỏ hàng
			var product = shoppingCart.FirstOrDefault(m => m.ProductID == productId);
			var productdb = ProductDataService.GetProduct(productId);
			if (product == null)
			{
				// Nếu sản phẩm chưa có trong giỏ, thêm mới vào giỏ
				product = new CartItem
				{
					ProductID = productId,
					ProductName = productdb.ProductName, // Thêm logic lấy tên sản phẩm từ database
					
					Unit = productdb.Unit, // Thêm logic lấy đơn vị từ database nếu có
					SalePrice = productdb.Price,// Thêm logic lấy giá từ database
					Quantity = quantity
				};
				shoppingCart.Add(product);
			}
			else
			{
				// Nếu sản phẩm đã có trong giỏ, cập nhật số lượng
				product.Quantity += quantity;
			}

			// Lưu giỏ hàng vào session
			ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
			int totalQuantity = shoppingCart.Sum(m => m.Quantity);
			return Json(new { Success = true, Message = "Sản phẩm đã được thêm vào giỏ hàng.", count = totalQuantity });
		}



		
		public IActionResult RemoveFromCart(int id = 0)
		{
			var shoppingCart = GetShoppingCart();
			int index = shoppingCart.FindIndex(m => m.ProductID == id);

			if (index >= 0)
			{
				shoppingCart.RemoveAt(index);
				ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

				return Json(new { Success = true, ShoppingCart = shoppingCart });
			}

			return Json(new { Success = false, errorMessage = "Mặt hàng không tồn tại trong giỏ hàng." });
		}
		


		public IActionResult ClearCart()
		{
			var shoppingCart = GetShoppingCart();
			shoppingCart.Clear();
			ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

			return Json(new { Success = true, ShoppingCart = shoppingCart });
		}



		

	}
}
