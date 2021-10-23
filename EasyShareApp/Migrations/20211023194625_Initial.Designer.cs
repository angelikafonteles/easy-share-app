﻿// <auto-generated />
using System;
using EasyShareApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EasyShareApp.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20211023194625_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EasyShareApp.Models.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte[]>("Attachment")
                        .HasColumnType("longblob");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("DownloadLimit")
                        .HasColumnType("int");

                    b.Property<bool>("DownloadLimitToggle")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Extension")
                        .HasColumnType("int");

                    b.Property<DateTime>("InstantCreation")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("InstantExpiration")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Path")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("RegisterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RegisterId");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("EasyShareApp.Models.Register", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Key")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2147483647);

                    b.HasKey("Id");

                    b.ToTable("Register");
                });

            modelBuilder.Entity("EasyShareApp.Models.Document", b =>
                {
                    b.HasOne("EasyShareApp.Models.Register", "Register")
                        .WithMany("Documents")
                        .HasForeignKey("RegisterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}