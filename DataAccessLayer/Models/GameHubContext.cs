using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Models;

public partial class GameHubContext : DbContext
{
    public GameHubContext()
    {
    }

    public GameHubContext(DbContextOptions<GameHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameCategory> GameCategories { get; set; }

    public virtual DbSet<Media> Media { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerLibrary> PlayerLibraries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\local;Uid=sa;Pwd=12345;Database=GameHub;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.DeveloperId).HasName("PK__Develope__DE084CF1D2A2E8BA");

            entity.ToTable("Developer");

            entity.HasIndex(e => e.DeveloperName, "UQ__Develope__08E3F54DDFC33C40").IsUnique();

            entity.Property(e => e.DeveloperName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Website).HasMaxLength(250);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__Game__2AB897FDCE930E1D");

            entity.ToTable("Game");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Games)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Game__CategoryId__300424B4");

            entity.HasOne(d => d.Developer).WithMany(p => p.Games)
                .HasForeignKey(d => d.DeveloperId)
                .HasConstraintName("FK__Game__DeveloperI__2F10007B");
        });

        modelBuilder.Entity<GameCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__GameCate__19093A0B9E800A62");

            entity.ToTable("GameCategory");

            entity.HasIndex(e => e.CategoryName, "UQ__GameCate__8517B2E03E7E1C51").IsUnique();

            entity.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Media>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("PK__Media__B2C2B5CF8F180F72");

            entity.Property(e => e.FileName).HasMaxLength(260);
            entity.Property(e => e.FilePath).HasMaxLength(260);
            entity.Property(e => e.Type).HasMaxLength(100);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Game).WithMany(p => p.Media)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MediaBinary_Game");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__Player__4A4E74C86E7B66AD");

            entity.ToTable("Player");

            entity.HasIndex(e => e.UserId, "UQ__Player__1788CC4DA9B2B63B").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Player__536C85E47A11270B").IsUnique();

            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.User).WithOne(p => p.Player)
                .HasForeignKey<Player>(d => d.UserId)
                .HasConstraintName("FK__Player__UserId__34C8D9D1");
        });

        modelBuilder.Entity<PlayerLibrary>(entity =>
        {
            entity.HasKey(e => e.PlayerLibraryId).HasName("PK__PlayerLi__28DE82FD550DBCC8");

            entity.ToTable("PlayerLibrary");

            entity.HasIndex(e => new { e.PlayerId, e.GameId }, "UQ_PlayerLibrary_Player_Game").IsUnique();

            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PurchasedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("owned");

            entity.HasOne(d => d.Game).WithMany(p => p.PlayerLibraries)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_PlayerLibrary_Game");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerLibraries)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("FK_PlayerLibrary_Player");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C17D7B6CE");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105347350A750").IsUnique();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
