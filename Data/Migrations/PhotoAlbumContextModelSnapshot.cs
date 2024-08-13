﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PhotoAlbumApi.Data;

#nullable disable

namespace Backend_dev.Data.Migrations
{
    [DbContext(typeof(PhotoAlbumContext))]
    partial class PhotoAlbumContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PhotoAlbumApi.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Albums");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Album for AI related photos",
                            Title = "AI"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Album for nature related photos",
                            Title = "Nature"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Album for travel related photos",
                            Title = "Travel"
                        },
                        new
                        {
                            Id = 4,
                            Description = "Album for family related photos",
                            Title = "Family"
                        },
                        new
                        {
                            Id = 5,
                            Description = "Album for food related photos",
                            Title = "Food"
                        });
                });

            modelBuilder.Entity("PhotoAlbumApi.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateUploaded")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(2083)
                        .HasColumnType("varchar(2083)");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("PhotoAlbumApi.Models.Photo", b =>
                {
                    b.HasOne("PhotoAlbumApi.Models.Album", null)
                        .WithMany("Photos")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PhotoAlbumApi.Models.Album", b =>
                {
                    b.Navigation("Photos");
                });
#pragma warning restore 612, 618
        }
    }
}
