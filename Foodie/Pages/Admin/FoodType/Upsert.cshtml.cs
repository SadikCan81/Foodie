﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodie.Pages.Admin.FoodType
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
        public Models.FoodType FoodTypeObj { get; set; }

        public IActionResult OnGet(int? id)
        {
            FoodTypeObj = new Models.FoodType();

            if(id != null)
            {
                FoodTypeObj = _unitOfWork.FoodType.GetFirstOrDefault(x => x.Id == id);
                if (FoodTypeObj == null)
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
                return Page();
            }

            if(FoodTypeObj.Id == 0)
            {
                _unitOfWork.FoodType.Add(FoodTypeObj);
            }
            else
            {
                _unitOfWork.FoodType.Update(FoodTypeObj);
            }

            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}