using System;
using System.Collections.Generic;

#nullable disable

namespace CashApp.Models
{
    public partial class CommandProduct
    {
        public int Id { get; set; }
        public int? IdProduct { get; set; }
        public int? IdCommand { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public virtual Command IdCommandNavigation { get; set; }
        public virtual Product IdProductNavigation { get; set; }
    }
}
