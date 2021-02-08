using System;
using System.Collections.Generic;
using System.Text;
using Foodie.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Foodie.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<FoodType> FoodTypes { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<UnitType> UnitTypes { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<MenuItemInventory> MenuItemInventories { get; set; }
    }
}
