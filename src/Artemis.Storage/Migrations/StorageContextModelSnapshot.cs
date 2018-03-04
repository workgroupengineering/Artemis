﻿// <auto-generated />

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Artemis.Storage.Migrations
{
    [DbContext(typeof(StorageContext))]
    internal class StorageContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Artemis.Storage.Entities.FolderEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<int?>("FolderEntityId");

                b.Property<string>("Name");

                b.Property<int>("Order");

                b.HasKey("Id");

                b.HasIndex("FolderEntityId");

                b.ToTable("Folders");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.KeypointEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<int?>("LayerSettingEntityId");

                b.Property<int>("Time");

                b.Property<string>("Value");

                b.HasKey("Id");

                b.HasIndex("LayerSettingEntityId");

                b.ToTable("Keypoints");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.LayerEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<int?>("FolderEntityId");

                b.Property<string>("Name");

                b.Property<int>("Order");

                b.Property<string>("Type");

                b.HasKey("Id");

                b.HasIndex("FolderEntityId");

                b.ToTable("Layers");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.LayerSettingEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<int?>("LayerEntityId");

                b.Property<string>("Name");

                b.Property<string>("Value");

                b.HasKey("Id");

                b.HasIndex("LayerEntityId");

                b.ToTable("LayerSettings");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.LedEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<int>("LayerId");

                b.Property<string>("LedName");

                b.Property<string>("LimitedToDevice");

                b.HasKey("Id");

                b.HasIndex("LayerId");

                b.ToTable("Leds");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.ProfileEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("Module");

                b.Property<string>("Name");

                b.Property<int>("RootFolderId");

                b.HasKey("Id");

                b.HasIndex("RootFolderId");

                b.ToTable("Profiles");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.SettingEntity", b =>
            {
                b.Property<string>("Name")
                    .ValueGeneratedOnAdd();

                b.Property<string>("Value");

                b.HasKey("Name");

                b.ToTable("Settings");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.FolderEntity", b =>
            {
                b.HasOne("Artemis.Storage.Entities.FolderEntity")
                    .WithMany("Folders")
                    .HasForeignKey("FolderEntityId");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.KeypointEntity", b =>
            {
                b.HasOne("Artemis.Storage.Entities.LayerSettingEntity")
                    .WithMany("Keypoints")
                    .HasForeignKey("LayerSettingEntityId");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.LayerEntity", b =>
            {
                b.HasOne("Artemis.Storage.Entities.FolderEntity")
                    .WithMany("Layers")
                    .HasForeignKey("FolderEntityId");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.LayerSettingEntity", b =>
            {
                b.HasOne("Artemis.Storage.Entities.LayerEntity")
                    .WithMany("Settings")
                    .HasForeignKey("LayerEntityId");
            });

            modelBuilder.Entity("Artemis.Storage.Entities.LedEntity", b =>
            {
                b.HasOne("Artemis.Storage.Entities.LayerEntity", "Layer")
                    .WithMany("Leds")
                    .HasForeignKey("LayerId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("Artemis.Storage.Entities.ProfileEntity", b =>
            {
                b.HasOne("Artemis.Storage.Entities.FolderEntity", "RootFolder")
                    .WithMany()
                    .HasForeignKey("RootFolderId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
#pragma warning restore 612, 618
        }
    }
}