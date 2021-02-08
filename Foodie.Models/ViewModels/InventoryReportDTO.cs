using System;
using System.Collections.Generic;
using System.Text;

namespace Foodie.Models.ViewModels
{
    public class InventoryReportDTO
    {
        public int? MenuItemId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
