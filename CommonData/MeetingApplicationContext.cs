using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CommonData.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommonData
{
    public class MeetingApplicationContext : IdentityDbContext<User, IdentityRole<int>, int>
	{
		public MeetingApplicationContext() // Mockoláshoz szükséges
		{ }

		public MeetingApplicationContext(DbContextOptions<MeetingApplicationContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<User>()
				.ToTable("Users");  // A felhasználói tábla alapértelmezett neve AspNetUsers lenne

			builder.Entity<EventForm>()
				.HasKey(e => new { e.MemberId, e.EventId });
			builder.Entity<MemberOfProject>()
				.HasKey(e => new { e.MemberId, e.ProjectId });
			builder.Entity<Membership>()
				.HasKey(e => new { e.MemberId, e.UserId });
			builder.Entity<AcceptedEmailDomain>()
				.HasKey(e => new { e.OrganisationId, e.DomainName });

			builder.Entity<EventForm>()
				.Property(e => e.IsFixGuest)
				.HasDefaultValue(false);

			builder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
			builder.Entity<Member>()
				.HasIndex(m => new { m.Email, m.OrganisationId })
				.IsUnique();
			builder.Entity<Organisation>()
				.HasIndex(o => o.Name)
				.IsUnique();

			builder.Entity<EventForm>()
				.HasOne(p => p.Member)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);
			builder.Entity<MemberOfProject>()
				.HasOne(p => p.Project)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);
			builder.Entity<MemberOfProject>()
				.HasOne(p => p.Member)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);
			builder.Entity<Member>()
				.HasOne(p => p.Organisation)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);
		}

		public virtual DbSet<AcceptedEmailDomain> AcceptedEmailDomains { get; set; }
		public virtual DbSet<Event> Events { get; set; }
		public virtual DbSet<EventForm> EventForms { get; set; }
		public virtual DbSet<Job> Jobs { get; set; }
		public virtual DbSet<Member> Members { get; set; }
		public virtual DbSet<Membership> Memberships { get; set; }
		public virtual DbSet<MemberOfProject> MemberOfProjects { get; set; }
		public virtual DbSet<Organisation> Organisations { get; set; }
		public virtual DbSet<Project> Projects { get; set; }
		public virtual DbSet<Venue> Venues { get; set; }
		public virtual DbSet<VenueImage> VenueImages { get; set; }

		/*
		 * A IdentityDbContext típustól további, az authentikációhoz és autorizációhoz kapcsolódó kollekciókat öröklünk, pl.:
		 * Users
		 * UserRoles
		 * stb.
		 */
	}
}
