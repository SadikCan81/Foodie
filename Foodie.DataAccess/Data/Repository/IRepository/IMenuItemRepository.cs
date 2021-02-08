using Foodie.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foodie.DataAccess.Data.Repository.IRepository
{
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        IEnumerable<SelectListItem> GetMenuItemListForDropdown();

        void Update(MenuItem menuItem);
    }
}
