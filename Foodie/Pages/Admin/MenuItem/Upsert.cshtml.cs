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
                MenuItem = new Models.MenuItem()
            };

            if (id != null)
            {
                menuItemObj.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(x => x.Id == id);
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

                return Page();
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

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
            }

            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}