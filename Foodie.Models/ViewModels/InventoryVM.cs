using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foodie.Models.ViewModels
{
    public class InventoryVM
    {
        public Inventory Inventory { get; set; }

        public IEnumerable<SelectListItem> UnitTypeList { get; set; }
    }
}
