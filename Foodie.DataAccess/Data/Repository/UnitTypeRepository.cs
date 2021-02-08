using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foodie.DataAccess.Data.Repository
{
    public class UnitTypeRepository : Repository<UnitType>, IUnitTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public UnitTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetUnitTypeListForDropdown()
        {
            return _db.UnitTypes.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }

        public void Update(UnitType unitType)
        {
            var objFromDb = _db.UnitTypes.FirstOrDefault(x => x.Id == unitType.Id);

            objFromDb.Name = unitType.Name;
            objFromDb.Description = unitType.Description;

            _db.SaveChanges();
        }
    }
}
