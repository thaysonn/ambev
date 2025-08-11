using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Serilog;


public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        const int maxRetries = 5;
        var delay = TimeSpan.FromSeconds(3);

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var pending = await db.Database.GetPendingMigrationsAsync();
                if (pending.Any())
                {
                    Log.Information("Applying {Count} migrations: {Migrations}", pending.Count(), string.Join(", ", pending));
                    await db.Database.MigrateAsync();
                    Log.Information("Migrations applied.");
                }
                else
                {
                    Log.Information("No pending migrations.");
                }
                return;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Migration attempt {Attempt}/{MaxRetries} failed.", attempt, maxRetries);
                if (attempt == maxRetries) throw;
                await Task.Delay(delay);
            }
        }
    }
}

