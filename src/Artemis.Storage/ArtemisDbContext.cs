using Artemis.Storage.Entities.General;
using Artemis.Storage.Entities.Plugins;
using Artemis.Storage.Entities.Profile;
using Artemis.Storage.Entities.Surface;
using Microsoft.EntityFrameworkCore;

namespace Artemis.Storage;

public class ArtemisDbContext : DbContext
{
    /// <inheritdoc />
    public ArtemisDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<DeviceEntity> Devices => Set<DeviceEntity>();
    public DbSet<PluginSettingEntity> PluginSettings => Set<PluginSettingEntity>();
    public DbSet<ProfileCategoryEntity> ProfileCategories => Set<ProfileCategoryEntity>();
    public DbSet<ProfileConfigurationEntity> ProfileConfigurations => Set<ProfileConfigurationEntity>();
    public DbSet<ProfileEntity> Profiles => Set<ProfileEntity>();
    public DbSet<QueuedActionEntity> QueuedActions => Set<QueuedActionEntity>();
    public DbSet<ReleaseEntity> Releases => Set<ReleaseEntity>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProfileEntity>().OwnsMany(e => e.Folders, builder => builder.ToJson());
        modelBuilder.Entity<ProfileEntity>().OwnsMany(e => e.Layers, builder => builder.ToJson());
        modelBuilder.Entity<ProfileEntity>().OwnsMany(e => e.ScriptConfigurations, builder => builder.ToJson());
    }

}