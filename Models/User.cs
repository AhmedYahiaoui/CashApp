﻿using System;
using System.Collections.Generic;

#nullable disable

namespace CashApp.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Banned { get; set; }
        public bool Active { get; set; }
        public string Role { get; set; }
    }
}
