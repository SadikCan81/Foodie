using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foodie.DataAccess.Data.Repository
{
    public class MenuItemInventoryRepository : Repository<MenuItemInventory>, IMenuItemInventoryRepository
    {
        private readonly ApplicationDbContext _db;

        public MenuItemInventoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(MenuItemInventory menuItemInventory)
        {
            var objFromDb = _db.MenuItemInventories.FirstOrDefault(x => x.Id == menuItemInventory.Id);

            objFromDb.MenuItemId = menuItemInventory.MenuItemId;
            objFromDb.InventoryId = menuItemInventory.InventoryId;
            objFromDb.Quantity = menuItemInventory.Quantity;

            _db.SaveChanges();
        }
    }
}
