using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Foodie.Models
{
    public class MenuItemInventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Display(Name = "Inventory")]
        public int InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public virtual Inventory Inventory { get; set; }

        [Display(Name = "Menu Item")]
        public int MenuItemId { get; set; }

        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
    }
}
