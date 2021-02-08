using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models.ViewModels;
using Foodie.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodie.Pages.Admin.InventoryReport
{
    [Authorize(Roles = SD.ManagerRole)]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InventoryReportVM InventoryReportVM { get; set; }

        public void OnGet()
        {
            InventoryReportVM = new InventoryReportVM()
            {
                MenuItemList = _unitOfWork.MenuItem.GetMenuItemListForDropdown()
            };
        }

        public IActionResult OnPost()
        {
            //Foreachleri tek seferde yapmaya calis

            if(InventoryReportVM.StartDate == DateTime.MinValue && InventoryReportVM.EndDate == DateTime.MinValue && InventoryReportVM.MenuItemId == null)
            {
                ViewData["Alert"] = "Please select start and end date at the same time or only a menu item! You can choose both three of them!";
                return Page();
            }

            var startDate = InventoryReportVM.StartDate == DateTime.MinValue ? new DateTime(2000, 1, 1) : InventoryReportVM.StartDate;
            var endDate = InventoryReportVM.EndDate == DateTime.MinValue ? DateTime.Now.AddYears(10) : InventoryReportVM.EndDate;

            InventoryReportVM.MenuItemList = _unitOfWork.MenuItem.GetMenuItemListForDropdown();

            if(InventoryReportVM.MenuItemId != null)
            {
                var inventoryList = new List<Models.Inventory>();

                foreach (var orderDetail in _unitOfWork.OrderDetail.GetAll(x => x.OrderDate <= endDate && x.OrderDate >= startDate && x.MenuItemId == InventoryReportVM.MenuItemId))
                {
                    foreach (var item in _unitOfWork.MenuItemInventory.GetAll(x => x.MenuItemId == orderDetail.MenuItemId, includeProperties: "Inventory"))
                    {
                        if(inventoryList.Where(x=> x.Name == item.Inventory.Name).Count() > 0)
                        {
                            var quantityForSameElement = item.Quantity * orderDetail.Count;
                            inventoryList.FirstOrDefault(x => x.Name == item.Inventory.Name).Quantity += quantityForSameElement;
                        }
                        else
                        {
                            var inventory = new Models.Inventory()
                            {
                                Name = item.Inventory.Name,
                                Quantity = item.Quantity * orderDetail.Count,
                                UnitType = _unitOfWork.UnitType.GetFirstOrDefault(x => x.Id == item.Inventory.UnitTypeId)
                            };

                            inventoryList.Add(inventory);
                        }
                        
                    }
                }

                InventoryReportVM.InventoryList = inventoryList;
                return Page();
            }
            else
            {
                var inventoryList = new List<Models.Inventory>();

                foreach (var orderDetail in _unitOfWork.OrderDetail.GetAll(x=> x.OrderDate <= InventoryReportVM.EndDate && x.OrderDate >= InventoryReportVM.StartDate))
                {
                    foreach (var item in _unitOfWork.MenuItemInventory.GetAll(x => x.MenuItemId == orderDetail.MenuItemId, includeProperties: "Inventory"))
                    {
                        if (inventoryList.Where(x => x.Name == item.Inventory.Name).Count() > 0)
                        {
                            var quantityForSameElement = item.Quantity * orderDetail.Count;
                            inventoryList.FirstOrDefault(x => x.Name == item.Inventory.Name).Quantity += quantityForSameElement;
                        }
                        else
                        {
                            var inventory = new Models.Inventory()
                            {
                                Name = item.Inventory.Name,
                                Quantity = item.Quantity * orderDetail.Count,
                                UnitType = _unitOfWork.UnitType.GetFirstOrDefault(x => x.Id == item.Inventory.UnitTypeId)
                            };

                            inventoryList.Add(inventory);
                        }
                    }
                }

                InventoryReportVM.InventoryList = inventoryList;
                return Page();
            }                        
        }
    }
}
