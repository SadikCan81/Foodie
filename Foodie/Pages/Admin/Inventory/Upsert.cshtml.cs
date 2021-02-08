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

namespace Foodie.Pages.Admin.Inventory
{
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InventoryVM InventoryVM { get; set; }

        public IActionResult OnGet(int? id)
        {
            InventoryVM = new InventoryVM()
            {
                Inventory = new Models.Inventory(),
                UnitTypeList = _unitOfWork.UnitType.GetUnitTypeListForDropdown()
            };

            if (id != null)
            {
                InventoryVM.Inventory = _unitOfWork.Inventory.GetFirstOrDefault(x => x.Id == id);

                if (InventoryVM.Inventory == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                InventoryVM.UnitTypeList = _unitOfWork.UnitType.GetUnitTypeListForDropdown();

                return Page();
            }

            if(InventoryVM.Inventory.Id == 0)
            {
                _unitOfWork.Inventory.Add(InventoryVM.Inventory);
            }
            else
            {
                var inventoryFromDb = _unitOfWork.Inventory.Get(InventoryVM.Inventory.Id);
                if(inventoryFromDb == null)
                {
                    return NotFound();
                }

                _unitOfWork.Inventory.Update(InventoryVM.Inventory);
            }

            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }
    }
}
