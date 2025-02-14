using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Entities;

public partial class Exam1Context : DbContext
{
    public Exam1Context(DbContextOptions<Exam1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BookedTicket> BookedTickets { get; set; }
    public virtual DbSet<BookedTicketTransaction> BookedTicketTransactions { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookedTicket>(entity =>
        {
            entity.HasKey(e => e.BookedTicketId).HasName("PK__BookedTi__9110472F6D335485");

            entity.ToTable("BookedTicket");

            entity.Property(e => e.BookedTicketId)
                .ValueGeneratedOnAdd(); // ✅ IDENTITY harus auto-generated

            entity.Property(e => e.BookedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.HasOne(d => d.BookedTicketTransaction)
                .WithMany(p => p.BookedTickets)
                .HasForeignKey(d => d.BookedTicketTransactionId)
                .OnDelete(DeleteBehavior.Cascade) // ✅ Jika transaksi dihapus, tiket juga ikut terhapus
                .HasConstraintName("FK__BookedTic__Booke__4222D4EF");

            entity.HasOne(d => d.TicketCodeNavigation)
                .WithMany(p => p.BookedTickets)
                .HasForeignKey(d => d.TicketCode)
                .OnDelete(DeleteBehavior.Cascade) // ✅ Jika tiket dihapus, bookingnya juga ikut
                .HasConstraintName("FK__BookedTic__Ticke__4316F928");
        });

        modelBuilder.Entity<BookedTicketTransaction>(entity =>
        {
            entity.HasKey(e => e.BookedTicketTransactionId).HasName("PK__BookedTi__EE0CA9DA0E9D6292");

            entity.ToTable("BookedTicketTransaction");

            entity.Property(e => e.BookedTicketTransactionId)
                .ValueGeneratedOnAdd(); // ✅ IDENTITY harus auto-generated

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SummaryPrice).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0BBBA1308E");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .ValueGeneratedOnAdd(); // ✅ IDENTITY harus auto-generated

            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketCode).HasName("PK__Ticket__598CF7A2B3CDFE2D");

            entity.ToTable("Ticket");

            entity.Property(e => e.TicketCode)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EventDateMaximal).HasColumnType("datetime");
            entity.Property(e => e.EventDateMinimal).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Quota).HasDefaultValue(0);

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade) // ✅ Jika kategori dihapus, tiket dalam kategori juga ikut terhapus
                .HasConstraintName("FK__Ticket__Category__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
