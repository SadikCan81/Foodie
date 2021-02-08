using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foodie.DataAccess.Data.Repository
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _db;

        public InventoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Inventory inventory)
        {
            var objFromDb = _db.Inventories.FirstOrDefault(x => x.Id == inventory.Id);

            objFromDb.Name = inventory.Name;
            objFromDb.Quantity = inventory.Quantity;
            objFromDb.UnitTypeId = inventory.UnitTypeId;

            _db.SaveChanges();
        }
    }
}
