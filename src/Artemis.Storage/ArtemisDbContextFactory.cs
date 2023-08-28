using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
// ReSharper disable UnusedType.Global

namespace Artemis.Storage;

// Used by design time tools
// https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-a-design-time-factory
public class ArtemisDbContextFactory : IDesignTimeDbContextFactory<ArtemisDbContext>
{
    public ArtemisDbContext CreateDbContext(string[] args)
    {
        string baseFolder = Environment.GetFolderPath(OperatingSystem.IsWindows()
            ? Environment.SpecialFolder.CommonApplicationData
            : Environment.SpecialFolder.LocalApplicationData);

        DbContextOptionsBuilder<ArtemisDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(baseFolder, "Artemis", "artemis.db")}");
        return new ArtemisDbContext(optionsBuilder.Options);
    }
}