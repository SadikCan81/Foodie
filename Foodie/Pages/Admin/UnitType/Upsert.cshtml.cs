using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foodie.DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodie.Pages.Admin.UnitType
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Models.UnitType UnitTypeObj { get; set; }

        public IActionResult OnGet(int? id)
        {
            UnitTypeObj = new Models.UnitType();

            if(id != null)
            {
                UnitTypeObj = _unitOfWork.UnitType.GetFirstOrDefault(x => x.Id == id);

                if (UnitTypeObj == null)
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

            if (UnitTypeObj.Id == 0)
            {
                _unitOfWork.UnitType.Add(UnitTypeObj);
            }
            else
            {
                _unitOfWork.UnitType.Update(UnitTypeObj);
            }

            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}
