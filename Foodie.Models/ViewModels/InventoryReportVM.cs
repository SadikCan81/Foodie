using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foodie.Models.ViewModels
{
    public class InventoryReportVM
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<SelectListItem> MenuItemList { get; set; }

        public int? MenuItemId { get; set; }

        public List<Inventory> InventoryList { get; set; }
    }
}
