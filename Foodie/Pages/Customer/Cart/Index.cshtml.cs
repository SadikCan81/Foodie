using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models;
using Foodie.Models.ViewModels;
using Foodie.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodie.Pages.Customer.Cart
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderDetailsCart OrderDetailsCartVM { get; set; }

        public void OnGet()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader(),
                ShoppingCartList = new List<ShoppingCart>()
            };

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);

                if (cart != null)
                {
                    OrderDetailsCartVM.ShoppingCartList = cart.ToList();
                }

                foreach (var cartList in OrderDetailsCartVM.ShoppingCartList)
                {
                    cartList.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(x => x.Id == cartList.MenuItemId);
                    OrderDetailsCartVM.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
                }
            }

        }

        public IActionResult OnPostPlus(int cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);

            shoppingCartFromDb.Count = _unitOfWork.ShoppingCart.IncrementCount(shoppingCartFromDb, 1);

            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/Index");

        }

        public IActionResult OnPostMinus(int cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);

            if (shoppingCartFromDb.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingCartFromDb.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCart, count);
            }
            else
            {
                shoppingCartFromDb.Count = _unitOfWork.ShoppingCart.DecrementCount(shoppingCartFromDb, 1);
                _unitOfWork.Save();
            }

            

            return RedirectToPage("/Customer/Cart/Index");

        }

        public IActionResult OnPostRemove(int cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);

            _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
            _unitOfWork.Save();

            var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingCartFromDb.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ShoppingCart, count);

            

            return RedirectToPage("/Customer/Cart/Index");
        }
    }
}