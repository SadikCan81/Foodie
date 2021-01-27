using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models;
using Foodie.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodie.Pages.Customer.Home
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public ShoppingCart ShoppingCartObj { get; set; }

        public void OnGet(int id)
        {
            ShoppingCartObj = new ShoppingCart
            {
                MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(filter: x => x.Id == id, includeProperties: "Category,FoodType"),
                MenuItemId = id
            };
        }

        public IActionResult OnPost()
        {
            if(!ModelState.IsValid)
            {
                ShoppingCartObj.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(filter: x => x.Id == ShoppingCartObj.MenuItemId, includeProperties: "Category,FoodType");
                return Page();
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartObj.ApplicationUserId = claim.Value;

            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ApplicationUserId == ShoppingCartObj.ApplicationUserId &&
                                                                            x.MenuItemId == ShoppingCartObj.MenuItemId);
            if(cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(ShoppingCartObj);
            }
            else
            {
                cartFromDb.Count = _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, ShoppingCartObj.Count);
            }
            _unitOfWork.Save();

            var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == ShoppingCartObj.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ShoppingCart, count);

            return RedirectToPage("Index");

        }
    }
}