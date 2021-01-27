using Foodie.DataAccess.Data.Repository.IRepository;
using Foodie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foodie.DataAccess.Data.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            var orderDetailFromDb = _db.OrderDetails.FirstOrDefault(m => m.Id == orderDetail.Id);
            _db.OrderDetails.Update(orderDetailFromDb);

            _db.SaveChanges();

        }
    }
}
