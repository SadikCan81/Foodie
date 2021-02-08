using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foodie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IActionResult Post([FromBody] InventoryReportDTO inventoryReportDTO)
        {
            if (inventoryReportDTO.StartDate == DateTime.MinValue && inventoryReportDTO.EndDate == DateTime.MinValue && inventoryReportDTO.MenuItemId == 0)
            {
                return BadRequest(new { message = "Please select start and end date at the same time or only a menu item! You can choose both three of them!" });
            }

            var startDateProp = inventoryReportDTO.StartDate == DateTime.MinValue ? new DateTime(2000, 1, 1) : inventoryReportDTO.StartDate;
            var endDateProp = inventoryReportDTO.EndDate == DateTime.MinValue ? DateTime.Now.AddYears(10) : inventoryReportDTO.EndDate;

            if (inventoryReportDTO.MenuItemId != null && inventoryReportDTO.MenuItemId != 0)
            {
                var inventoryList = new List<Models.Inventory>();

                foreach (var orderDetail in _unitOfWork.OrderDetail.GetAll(x => x.OrderDate <= endDateProp && x.OrderDate >= startDateProp && x.MenuItemId == inventoryReportDTO.MenuItemId))
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

                return Ok(inventoryList);
            }
            else
            {
                var inventoryList = new List<Models.Inventory>();

                foreach (var orderDetail in _unitOfWork.OrderDetail.GetAll(x => x.OrderDate <= endDateProp && x.OrderDate >= startDateProp))
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

                return Ok(inventoryList);
            }
        }
    }
}
