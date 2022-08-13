using System;
using System.Collections.Generic;

#nullable disable

namespace CashApp.Models
{
    public partial class Command
    {
        public Command()
        {
            CommandProducts = new HashSet<CommandProduct>();
        }

        public int Id { get; set; }
        public double Price { get; set; }
        public string Ref { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Products { get; set; }
        public int UserId { get; set; }

        public virtual ICollection<CommandProduct> CommandProducts { get; set; }
    }
}
