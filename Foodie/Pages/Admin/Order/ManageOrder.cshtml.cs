using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models;
using Foodie.Models.ViewModels;
using Foodie.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace Foodie.Pages.Admin.Order
{
    public class ManageOrderModel : PageModel
    {
        private readonly IUnitOfWork _unitOfwork;

        public ManageOrderModel(IUnitOfWork unitOfWork)
        {
            _unitOfwork = unitOfWork;
        }

        [BindProperty]
        public List<OrderDetailsVM> OrderDetailsListVM { get; set; }

        public void OnGet()
        {
            List<OrderHeader> orderHeadersList = new List<OrderHeader>();
            OrderDetailsListVM = new List<OrderDetailsVM>();

            orderHeadersList = _unitOfwork.OrderHeader.GetAll(x=> x.Status == SD.StatusSubmitted || x.Status == SD.StatusInProcess).OrderBy(x=> x.PickUpTime).ToList();

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsVM indivual = new OrderDetailsVM
                {
                    OrderHeader = orderHeader,
                    OrderDetails = _unitOfwork.OrderDetail.GetAll(x => x.OrderHeaderId == orderHeader.Id).ToList()
                };

                OrderDetailsListVM.Add(indivual);
            }

        }

        public IActionResult OnPostOrderPrepare(int orderId)
        {
            OrderHeader orderHeader = _unitOfwork.OrderHeader.GetFirstOrDefault(x => x.Id == orderId);

            orderHeader.Status = SD.StatusInProcess;
            _unitOfwork.Save();

            return RedirectToPage();
        }

        public IActionResult OnPostOrderReady(int orderId)
        {
            OrderHeader orderHeader = _unitOfwork.OrderHeader.GetFirstOrDefault(x => x.Id == orderId);

            orderHeader.Status = SD.StatusReady;
            _unitOfwork.Save();

            return RedirectToPage();
        }

        public IActionResult OnPostOrderCancel(int orderId)
        {
            OrderHeader orderHeader = _unitOfwork.OrderHeader.GetFirstOrDefault(x => x.Id == orderId);

            orderHeader.Status = SD.StatusCancelled;
            _unitOfwork.Save();

            return RedirectToPage();
        }

        public IActionResult OnPostOrderRefund(int orderId)
        {
            OrderHeader orderHeader = _unitOfwork.OrderHeader.GetFirstOrDefault(x => x.Id == orderId);

            var options = new RefundCreateOptions
            {
                Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                Reason = RefundReasons.RequestedByCustomer,
                Charge = orderHeader.TransactionId
            };
            var service = new RefundService();
            Refund refund = service.Create(options);


            orderHeader.Status = SD.StatusRefunded;
            _unitOfwork.Save();

            return RedirectToPage();
        }
    }
}