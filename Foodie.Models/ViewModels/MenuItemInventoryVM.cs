using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foodie.Models.ViewModels
{
    public class MenuItemInventoryVM
    {        
        public int? MenuItemInventoryId { get; set; }

        [Required]
        public double? Quantity { get; set; }
        
        public int? InventoryId { get; set; }
        
        public int? MenuItemId { get; set; }

        public string InventoryName { get; set; }

        public string UnitTypeName { get; set; }
    }
}
