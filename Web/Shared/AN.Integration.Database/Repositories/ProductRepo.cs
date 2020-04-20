﻿using System.Data.SqlClient;
using AN.Integration.Database.Models.Models;

namespace AN.Integration.Database.Repositories
{
    public sealed class ProductRepo: TableRepo<Product>
    {
        public ProductRepo(SqlConnection connection) : base(connection)
        {
        }
    }
}