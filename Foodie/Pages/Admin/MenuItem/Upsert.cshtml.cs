using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models.ViewModels;
using Foodie.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodie.Pages.Admin.MenuItem
{
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UpsertModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnviroment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnviroment;
        }

        [BindProperty]
        public MenuItemVM menuItemObj { get; set; }

        public IActionResult OnGet(int? id)
        {
            menuItemObj = new MenuItemVM
            {
                CategoryList = _unitOfWork.Category.GetCategoryListForDropdown(),
                FoodTypeList = _unitOfWork.FoodType.GetFoodTypeListForDropdown(),
                MenuItem = new Models.MenuItem(),
                InventoryList = _unitOfWork.Inventory.GetAll(includeProperties: "UnitType").ToList(),
                MenuItemInventories = new List<MenuItemInventoryVM>()
            };

            if (id != null)
            {
                menuItemObj.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(x => x.Id == id);
                //SQL LEFT JOIN Duzenle????!!!

                menuItemObj.MenuItemInventories = (from inventory in _unitOfWork.Inventory.GetAll(includeProperties: "UnitType")
                                                  join menuItemInventory in _unitOfWork.MenuItemInventory.GetAll()
                                                  on inventory.Id equals menuItemInventory.InventoryId into fmInventory
                                                  from fullMenuInventory in fmInventory.DefaultIfEmpty()
                                                  where ((fullMenuInventory == null ? 0 : fullMenuInventory.MenuItemId) == id) || ((fullMenuInventory == null ? 0 : fullMenuInventory.Quantity) == 0)
                                                  select new MenuItemInventoryVM()
                                                  {
                                                      MenuItemInventoryId = fullMenuInventory == null ? 0 : fullMenuInventory.Id,
                                                      InventoryId = inventory.Id,
                                                      MenuItemId = fullMenuInventory == null ? 0 : fullMenuInventory.MenuItemId,
                                                      InventoryName = inventory.Name,
                                                      UnitTypeName = inventory.UnitType.Name,
                                                      Quantity = fullMenuInventory == null ? 0 : fullMenuInventory.Quantity
                                                  }).ToList();
  
                if (menuItemObj.MenuItem == null)
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
                menuItemObj.CategoryList = _unitOfWork.Category.GetCategoryListForDropdown();
                menuItemObj.FoodTypeList = _unitOfWork.FoodType.GetFoodTypeListForDropdown();
                menuItemObj.InventoryList = _unitOfWork.Inventory.GetAll(includeProperties: "UnitType").ToList();                
                menuItemObj.MenuItemInventories = (from inventory in _unitOfWork.Inventory.GetAll(includeProperties: "UnitType")
                                                   join menuItemInventory in _unitOfWork.MenuItemInventory.GetAll()
                                                   on inventory.Id equals menuItemInventory.InventoryId into fmInventory
                                                   from fullMenuInventory in fmInventory.DefaultIfEmpty()
                                                   where ((fullMenuInventory == null ? 0 : fullMenuInventory.MenuItemId) == menuItemObj.MenuItem.Id) || ((fullMenuInventory == null ? 0 : fullMenuInventory.Quantity) == 0)
                                                   select new MenuItemInventoryVM()
                                                   {
                                                       MenuItemInventoryId = fullMenuInventory == null ? 0 : fullMenuInventory.Id,
                                                       InventoryId = inventory.Id,
                                                       MenuItemId = fullMenuInventory == null ? 0 : fullMenuInventory.MenuItemId,
                                                       InventoryName = inventory.Name,
                                                       UnitTypeName = inventory.UnitType.Name,
                                                       Quantity = fullMenuInventory == null ? 0 : fullMenuInventory.Quantity
                                                   }).ToList();

                return Page();
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var AreChecked = Request.Form["AreChecked"].ToList();

            if (menuItemObj.MenuItem.Id == 0)
            {
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(webRootPath, @"images\menuItems");
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                menuItemObj.MenuItem.Image = @"\images\menuItems\" + fileName + extension;

                _unitOfWork.MenuItem.Add(menuItemObj.MenuItem);
                _unitOfWork.Save();

                foreach (var item in menuItemObj.InventoryList.Where(x=> x.Quantity > 0))
                {
                    var menuItemInventory = new Models.MenuItemInventory();
                    
                    menuItemInventory.InventoryId = item.Id;
                    menuItemInventory.MenuItemId = menuItemObj.MenuItem.Id;
                    menuItemInventory.Quantity = item.Quantity;

                    _unitOfWork.MenuItemInventory.Add(menuItemInventory);
                }

            }

            else
            {
                var objFromDb = _unitOfWork.MenuItem.Get(menuItemObj.MenuItem.Id);

                if (objFromDb == null)
                {
                    return NotFound();
                }
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    string uploads = Path.Combine(webRootPath, @"images\menuItems");
                    string extension = Path.GetExtension(files[0].FileName);

                    var imagePath = Path.Combine(webRootPath, objFromDb.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    menuItemObj.MenuItem.Image = @"\images\menuItems\" + fileName + extension;
                }
                else
                {
                    menuItemObj.MenuItem.Image = objFromDb.Image;
                }

                _unitOfWork.MenuItem.Update(menuItemObj.MenuItem);

                foreach (var item in menuItemObj.MenuItemInventories)
                {
                    if(item.MenuItemInventoryId != null && item.MenuItemInventoryId > 0)
                    {
                        var menuItemInventoryFromDb = _unitOfWork.MenuItemInventory.GetFirstOrDefault(x => x.Id == item.MenuItemInventoryId);
                        if(menuItemInventoryFromDb != null && menuItemInventoryFromDb.Quantity != item.Quantity)
                        {
                            menuItemInventoryFromDb.Quantity = item.Quantity.GetValueOrDefault();
                            _unitOfWork.MenuItemInventory.Update(menuItemInventoryFromDb);
                        }                        
                    }
                    else
                    {
                        if (item.Quantity > 0)
                        {
                            var menuItemInventoryObj = new Models.MenuItemInventory()
                            {
                                MenuItemId = menuItemObj.MenuItem.Id,
                                InventoryId = item.InventoryId.GetValueOrDefault(),
                                Quantity = item.Quantity.GetValueOrDefault()
                            };

                            _unitOfWork.MenuItemInventory.Add(menuItemInventoryObj);
                        }
                    }
                }
            }

            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}