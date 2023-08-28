using System.IO;
using System.Reflection;
using Artemis.Storage.Repositories.Interfaces;
using DryIoc;
using Microsoft.EntityFrameworkCore;

namespace Artemis.Storage.DryIoc;

public static class ContainerExtensions
{
    /// <summary>
    ///     Registers storage services into the container.
    /// </summary>
    /// <param name="container">The builder building the current container</param>
    /// <param name="dataFolder">The data folder.</param>
    public static void RegisterStorage(this IContainer container, string dataFolder)
    {
        Assembly[] storageAssembly = {typeof(IRepository).Assembly};

        // Bind storage
        container.RegisterDelegate(() => new ArtemisDbContext(new DbContextOptionsBuilder().UseSqlite($"Data Source={Path.Join(dataFolder, "artemis.db")}").Options), Reuse.Singleton);
        container.RegisterMany(storageAssembly, type => type.IsAssignableTo<IRepository>(), Reuse.Singleton);
    }
}