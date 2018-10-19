namespace FIT5032_Assignment_New.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Maps : DbContext
    {
        public Maps()
            : base("name=Maps")
        {
        }

        public virtual DbSet<Invitee> Invitees { get; set; }
        public virtual DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invitee>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Invitee>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.LocationName)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.Latitude)
                .HasPrecision(10, 8);

            modelBuilder.Entity<Location>()
                .Property(e => e.Longitude)
                .HasPrecision(11, 8);

            modelBuilder.Entity<Location>()
                .Property(e => e.InviterId)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .HasMany(e => e.Invitees)
                .WithRequired(e => e.Location)
                .HasForeignKey(e => e.InvitationId)
                .WillCascadeOnDelete(false);
        }
    }
}
