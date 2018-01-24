using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using URF.Core.EF.Tests.Models;

namespace URF.Core.EF.Tests.Contexts
{
    internal static class NorthwindDbContextSeed
    {
        public static void SeedDataSql(this NorthwindDbContext context,
            IEnumerable<Category> categories, IEnumerable<Product> products)
        {
            try
            {
                context.Database.OpenConnection();
                context.Categories.AddRange(categories);
                context.Database.ExecuteSqlCommand("DELETE dbo.Categories");
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Categories ON");
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Categories OFF");

                context.Products.AddRange(products);
                context.Database.ExecuteSqlCommand("DELETE dbo.Products");
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Products ON");
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Products OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }
    }
}