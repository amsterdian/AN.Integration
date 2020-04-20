﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AN.Integration.Database.Models.Models
{
    [Table("Accounts", Schema = "dbo")]
    public class Account : IDatabaseTable
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
