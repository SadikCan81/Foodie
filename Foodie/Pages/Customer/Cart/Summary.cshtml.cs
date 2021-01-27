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
using Stripe;

namespace Foodie.Pages.Customer.Cart
{
    [Authorize]
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public OrderDetailsCart OrderDetailsCartVM { get; set; }
        public IActionResult OnGet()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            

            if(claim.Value != null)
            {
                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);
                IEnumerable<ShoppingCart> shoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value);

                if(shoppingCartList != null)
                {
                    OrderDetailsCartVM.ShoppingCartList = shoppingCartList.ToList();
                }

                foreach (var shoppingCart in OrderDetailsCartVM.ShoppingCartList)
                {
                    shoppingCart.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(x => x.Id == shoppingCart.MenuItemId);
                    OrderDetailsCartVM.OrderHeader.OrderTotal += (shoppingCart.MenuItem.Price * shoppingCart.Count);
                }

                OrderDetailsCartVM.OrderHeader.PickupName = applicationUser.FullName;
                OrderDetailsCartVM.OrderHeader.PickUpTime = DateTime.Now;
                OrderDetailsCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
                
            }

            return Page();
        }

        public IActionResult OnPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList();
            
            OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCartVM.OrderHeader.ApplicationUserId = claim.Value;
            OrderDetailsCartVM.OrderHeader.Status = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.PickUpTime = Convert.ToDateTime(OrderDetailsCartVM.OrderHeader.PickUpDate.ToShortDateString() + " " + OrderDetailsCartVM.OrderHeader.PickUpTime.ToShortTimeString());

            _unitOfWork.OrderHeader.Add(OrderDetailsCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var shoppingCart in OrderDetailsCartVM.ShoppingCartList)
            {
                shoppingCart.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(x => x.Id == shoppingCart.MenuItemId);

                OrderDetail orderDetail = new OrderDetail
                {
                    MenuItemId = shoppingCart.MenuItemId,
                    OrderHeaderId = OrderDetailsCartVM.OrderHeader.Id,
                    Name = shoppingCart.MenuItem.Name,
                    Count = shoppingCart.Count,
                    Price = shoppingCart.MenuItem.Price,
                    Description = shoppingCart.MenuItem.Description
                };

                OrderDetailsCartVM.OrderHeader.OrderTotal += (orderDetail.Count * orderDetail.Price);
                _unitOfWork.OrderDetail.Add(orderDetail);
            }

            OrderDetailsCartVM.OrderHeader.OrderTotal = Convert.ToDouble(String.Format("{0:.##}", OrderDetailsCartVM.OrderHeader.OrderTotal));
            _unitOfWork.ShoppingCart.RemoveRange(OrderDetailsCartVM.ShoppingCartList);
            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
            _unitOfWork.Save();

            if(stripeToken != null)
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(OrderDetailsCartVM.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Source = stripeToken,
                    Description = "Order ID: " + OrderDetailsCartVM.OrderHeader.Id,
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);

                OrderDetailsCartVM.OrderHeader.TransactionId = charge.Id;

                if (charge.Status.ToLower() == "succeeded")
                {
                    //email 
                    OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    OrderDetailsCartVM.OrderHeader.Status = SD.StatusSubmitted;
                }
                else
                {
                    OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = OrderDetailsCartVM.OrderHeader.Id });
        }
    }
}