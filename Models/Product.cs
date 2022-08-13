using System;
using System.Collections.Generic;

#nullable disable

namespace CashApp.Models
{
    public partial class Product
    {
        public Product()
        {
            CommandProducts = new HashSet<CommandProduct>();
        }

        public int Id { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Ref { get; set; }
        public string Dimension { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<CommandProduct> CommandProducts { get; set; }
    }
}
