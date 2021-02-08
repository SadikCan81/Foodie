using Foodie.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foodie.DataAccess.Data.Repository.IRepository
{
    public interface IUnitTypeRepository : IRepository<UnitType>
    {
        IEnumerable<SelectListItem> GetUnitTypeListForDropdown();

        void Update(UnitType unitType);
    }
}
