﻿// <auto-generated />
using System;
using CollectIt.MVC.Resources.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CollectIt.MVC.View.Migrations.Resources
{
    [DbContext(typeof(PostgresqlResourcesDbContext))]
    [Migration("20220320163804_AddSourcesConstraints")]
    partial class AddSourcesConstraints
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CollectIt.MVC.Account.IdentityEntities.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AppliedResourceType")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MaxResourcesCount")
                        .HasColumnType("integer");

                    b.Property<int>("MonthDuration")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Subscription");
                });

            modelBuilder.Entity("CollectIt.MVC.Account.IdentityEntities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CommentId"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<int>("TargetResourceId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("CommentId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("TargetResourceId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Image", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ImageId"));

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer");

                    b.HasKey("ImageId");

                    b.HasIndex("ResourceId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Music", b =>
                {
                    b.Property<int>("MusicId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MusicId"));

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval");

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer");

                    b.HasKey("MusicId");

                    b.HasIndex("ResourceId");

                    b.ToTable("Musics");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Resource", b =>
                {
                    b.Property<int>("ResourceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ResourceId"));

                    b.Property<int>("ResourceOwnerId")
                        .HasColumnType("integer");

                    b.Property<string>("ResourcePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ResourceId");

                    b.HasIndex("ResourceOwnerId");

                    b.ToTable("Resource");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Video", b =>
                {
                    b.Property<int>("VideoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("VideoId"));

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval");

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer");

                    b.HasKey("VideoId");

                    b.HasIndex("ResourceId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("SubscriptionUser", b =>
                {
                    b.Property<int>("SubscribersId")
                        .HasColumnType("integer");

                    b.Property<int>("SubscriptionsId")
                        .HasColumnType("integer");

                    b.HasKey("SubscribersId", "SubscriptionsId");

                    b.HasIndex("SubscriptionsId");

                    b.ToTable("SubscriptionUser");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Comment", b =>
                {
                    b.HasOne("CollectIt.MVC.Account.IdentityEntities.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CollectIt.MVC.Resources.Entities.Resource", "Target")
                        .WithMany()
                        .HasForeignKey("TargetResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Image", b =>
                {
                    b.HasOne("CollectIt.MVC.Resources.Entities.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Music", b =>
                {
                    b.HasOne("CollectIt.MVC.Resources.Entities.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Resource", b =>
                {
                    b.HasOne("CollectIt.MVC.Account.IdentityEntities.User", "ResourceOwner")
                        .WithMany()
                        .HasForeignKey("ResourceOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ResourceOwner");
                });

            modelBuilder.Entity("CollectIt.MVC.Resources.Entities.Video", b =>
                {
                    b.HasOne("CollectIt.MVC.Resources.Entities.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("SubscriptionUser", b =>
                {
                    b.HasOne("CollectIt.MVC.Account.IdentityEntities.User", null)
                        .WithMany()
                        .HasForeignKey("SubscribersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CollectIt.MVC.Account.IdentityEntities.Subscription", null)
                        .WithMany()
                        .HasForeignKey("SubscriptionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
