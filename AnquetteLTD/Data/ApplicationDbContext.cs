using AnquetteLTD.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnquetteLTD.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Anquette> Anquettes => Set<Anquette>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<AnquetteSubmission> Submissions => Set<AnquetteSubmission>();
        public DbSet<SubmissionAnswer> SubmissionAnswers => Set<SubmissionAnswer>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Answer
            builder.Entity<Answer>()
                .HasOne(a => a.Anquette)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.AnquetteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Anquette
            builder.Entity<Anquette>()
                .HasOne(q => q.User)
                .WithMany()
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // AnquetteSubmissions
            builder.Entity<AnquetteSubmission>()
                          .HasOne(s => s.Anquette)
                          .WithMany(q => q.Submissions)
                          .HasForeignKey(s => s.AnquetteId)
                          .OnDelete(DeleteBehavior.Cascade);
            // Submission
            builder.Entity<SubmissionAnswer>()
                .HasOne(sa => sa.Submission)
                .WithMany(s => s.SelectedAnswers)
                .HasForeignKey(sa => sa.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // SubmissionAnswer
            builder.Entity<SubmissionAnswer>()
                .HasOne(sa => sa.Answer)
                .WithMany()
                .HasForeignKey(sa => sa.AnswerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
