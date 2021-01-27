using System;
using System.Collections.Generic;
using System.Text;

namespace Foodie.Models.ViewModels
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> ShoppingCartList { get; set; }

        public OrderHeader OrderHeader { get; set; }
    }
}
