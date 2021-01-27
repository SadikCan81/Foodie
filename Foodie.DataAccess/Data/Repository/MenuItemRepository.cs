using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foodie.DataAccess.Data.Repository
{
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _db;

        public MenuItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(MenuItem menuItem)
        {
            var menuItemFromDb = _db.MenuItems.FirstOrDefault(x => x.Id == menuItem.Id);

            menuItemFromDb.Name = menuItem.Name;
            menuItemFromDb.Description = menuItem.Description;
            menuItemFromDb.Price = menuItem.Price;
            menuItemFromDb.FoodTypeId = menuItem.FoodTypeId;
            menuItemFromDb.CategoryId = menuItem.CategoryId;
            if(menuItem.Image != null)
            {
                menuItemFromDb.Image = menuItem.Image;
            }
            
            _db.SaveChanges();
        }
    }
}
