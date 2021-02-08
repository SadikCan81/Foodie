using Foodie.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foodie.DataAccess.Data.Repository.IRepository
{
    public interface IMenuItemInventoryRepository : IRepository<MenuItemInventory>
    {
        void Update(MenuItemInventory menuItemInventory);
    }
}
