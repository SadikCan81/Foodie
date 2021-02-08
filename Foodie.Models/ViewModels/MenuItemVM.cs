using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foodie.Models.ViewModels
{
    public class MenuItemVM
    {
        public MenuItem MenuItem { get; set; }

        public List<Inventory> InventoryList { get; set; }

        public List<MenuItemInventoryVM> MenuItemInventories { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; }

        public IEnumerable<SelectListItem> FoodTypeList { get; set; }
    }
}
