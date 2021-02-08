using Foodie.DataAccess.Data.Repository.IRepository;
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
    public class UnitTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnitTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.UnitType.GetAll() });
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.UnitType.GetFirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.UnitType.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
