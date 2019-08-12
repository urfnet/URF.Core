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
                context.Database.ExecuteSqlRaw("DELETE Categories");
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON");
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories OFF");

                context.Products.AddRange(products);
                context.Database.ExecuteSqlRaw("DELETE Products");
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }
    }
}