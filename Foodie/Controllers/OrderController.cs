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

namespace Foodie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       
        [HttpGet]
        [Authorize]
        public IActionResult OnGet(string status = null)
        {
            List<OrderDetailsVM> orderListVM = new List<OrderDetailsVM>();

            List<OrderHeader> OrderHeaderList = new List<OrderHeader>();

            if (User.IsInRole(SD.CustomerRole))
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if(claim != null)
                {
                    OrderHeaderList = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == claim.Value,null, "ApplicationUser").ToList();
                }
            }
            else
            {
                OrderHeaderList = _unitOfWork.OrderHeader.GetAll(null, null, "ApplicationUser").ToList();
            }

            if(status == "cancelled")
            {
                OrderHeaderList = OrderHeaderList.Where(x => x.Status == SD.StatusCancelled || x.Status == SD.StatusRefunded || x.Status == SD.PaymentStatusRejected).ToList();
            }
            else if(status == "completed")
            {
                OrderHeaderList = OrderHeaderList.Where(x => x.Status == SD.StatusCompleted).ToList();
            }
            else
            {
                OrderHeaderList = OrderHeaderList.Where(o => o.Status == SD.StatusReady || o.Status == SD.StatusInProcess || o.Status == SD.StatusSubmitted || o.Status == SD.PaymentStatusPending).ToList();
            }

            foreach (var orderHeader in OrderHeaderList)
            {
                OrderDetailsVM individual = new OrderDetailsVM()
                {
                    OrderHeader = orderHeader,
                    OrderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderHeader.Id).ToList()
                };
                orderListVM.Add(individual);
            }

            return Json(new { data = orderListVM });
        }
    }
}