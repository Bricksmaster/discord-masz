﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MASZ.Data;

namespace MASZ.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20210824161028_RemovePunishmentProperty")]
    partial class RemovePunishmentProperty
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("masz.Models.APIToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<byte[]>("TokenHash")
                        .HasColumnType("longblob");

                    b.Property<byte[]>("TokenSalt")
                        .HasColumnType("longblob");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("APITokens");
                });

            modelBuilder.Entity("masz.Models.AutoModerationConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AutoModerationAction")
                        .HasColumnType("int");

                    b.Property<int>("AutoModerationType")
                        .HasColumnType("int");

                    b.Property<string>("CustomWordFilter")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("IgnoreChannels")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("IgnoreRoles")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("Limit")
                        .HasColumnType("int");

                    b.Property<int?>("PunishmentDurationMinutes")
                        .HasColumnType("int");

                    b.Property<int?>("PunishmentType")
                        .HasColumnType("int");

                    b.Property<bool>("SendDmNotification")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("SendPublicNotification")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("TimeLimitMinutes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AutoModerationConfigs");
                });

            modelBuilder.Entity("masz.Models.AutoModerationEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AssociatedCaseId")
                        .HasColumnType("int");

                    b.Property<int>("AutoModerationAction")
                        .HasColumnType("int");

                    b.Property<int>("AutoModerationType")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Discriminator")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("MessageContent")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("MessageId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Nickname")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Username")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("AutoModerationEvents");
                });

            modelBuilder.Entity("masz.Models.CaseTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CaseDescription")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("CaseLabels")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CasePunishedUntil")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CasePunishmentType")
                        .HasColumnType("int");

                    b.Property<string>("CaseTitle")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreatedForGuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("TemplateName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("ViewPermission")
                        .HasColumnType("int");

                    b.Property<bool>("announceDm")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("handlePunishment")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("sendPublicNotification")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("CaseTemplates");
                });

            modelBuilder.Entity("masz.Models.GuildConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AdminRoles")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("ExecuteWhoisOnJoin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ModInternalNotificationWebhook")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("ModNotificationDM")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModPublicNotificationWebhook")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ModRoles")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("MutedRoles")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("PublishModeratorInfo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<bool>("StrictModPermissionCheck")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("GuildConfigs");
                });

            modelBuilder.Entity("masz.Models.GuildMotd", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Message")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("ShowMotd")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("GuildMotds");
                });

            modelBuilder.Entity("masz.Models.ModCase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("AllowComments")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<int>("CaseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreationType")
                        .HasColumnType("int");

                    b.Property<string>("DeletedByUserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Discriminator")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("GuildId")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Labels")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("LastEditedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LastEditedByModId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("LockedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LockedByUserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("MarkedToDeleteAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Nickname")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("OccuredAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Others")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("PunishedUntil")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("PunishmentActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("PunishmentType")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Username")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Valid")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("ModCases");
                });

            modelBuilder.Entity("masz.Models.ModCaseComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Message")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("ModCaseId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("ModCaseId");

                    b.ToTable("ModCaseComments");
                });

            modelBuilder.Entity("masz.Models.UserInvite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("InviteCreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("InviteIssuerId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("JoinedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("JoinedUserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("TargetChannelId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UsedInvite")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("UserInvites");
                });

            modelBuilder.Entity("masz.Models.UserMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreatorUserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Reason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserA")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserB")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("UserMappings");
                });

            modelBuilder.Entity("masz.Models.UserNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CreatorId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("UserNotes");
                });

            modelBuilder.Entity("masz.Models.ModCaseComment", b =>
                {
                    b.HasOne("masz.Models.ModCase", "ModCase")
                        .WithMany("Comments")
                        .HasForeignKey("ModCaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
