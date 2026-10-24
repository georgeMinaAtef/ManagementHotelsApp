using HotelApp.Domain.Common;
using HotelApp.Domain.Contracts;
using HotelApp.Infrastructure.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace HotelApp.Infrastructure.DataSeeding
{
    internal class CatalogDataSeeder(HotelIdentityDbContext dbContext, ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var PendingMigration = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
                if (PendingMigration.Any())
                    await dbContext.Database.MigrateAsync(cancellationToken);

                var seedRoot = Path.Combine(AppContext.BaseDirectory, "DataSeeding");

            
                int result = await dbContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                    logger.LogInformation($"{result} Rows Added ");
                else
                    logger.LogInformation("Database Already Seeding");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Seeding Failed");
            }
        }

        private async Task SeedIfEmptyAsync<T, TKey>(string rootPath, string fileName, CancellationToken c = default) where T : BaseEntity<TKey>
        {
            if (await dbContext.Set<T>().AnyAsync(c))
            {
                logger.LogInformation("Table Already Has Data ");
                return;
            }
            var filePath = Path.Combine(rootPath, fileName);
            if (!File.Exists(filePath))
            {
                logger.LogError($"File {fileName} Not Found at {filePath}");
                return;
            }
            using var fileStream = File.OpenRead(filePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
            var items = await JsonSerializer.DeserializeAsync<List<T>>(fileStream, options, c);

            if (items?.Any() ?? false)
                dbContext.Set<T>().AddRange(items!);
        }
    }
}