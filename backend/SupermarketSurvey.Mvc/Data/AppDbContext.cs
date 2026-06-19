using Microsoft.EntityFrameworkCore;
using SupermarketSurvey.Mvc.Models.Entities;

namespace SupermarketSurvey.Mvc.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Merchandiser> Merchandisers => Set<Merchandiser>();
    public DbSet<Outlet> Outlets => Set<Outlet>();
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<Respondent> Respondents => Set<Respondent>();
    public DbSet<SurveyResponse> SurveyResponses => Set<SurveyResponse>();
    public DbSet<SurveyResponseDetail> SurveyResponseDetails => Set<SurveyResponseDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Merchandiser>(e =>
        {
            e.HasIndex(m => m.EmployeeId).IsUnique();
            e.HasIndex(m => m.Email).IsUnique();
            e.HasOne(m => m.AssignedOutlet)
             .WithMany(o => o.Merchandisers)
             .HasForeignKey(m => m.AssignedOutletId);
        });

        modelBuilder.Entity<Outlet>(e =>
            e.HasIndex(o => o.BranchCode).IsUnique());

        modelBuilder.Entity<Question>(e =>
            e.HasOne(q => q.Survey).WithMany(s => s.Questions)
             .HasForeignKey(q => q.SurveyId).OnDelete(DeleteBehavior.Cascade));

        modelBuilder.Entity<AnswerOption>(e =>
            e.HasOne(ao => ao.Question).WithMany(q => q.AnswerOptions)
             .HasForeignKey(ao => ao.QuestionId).OnDelete(DeleteBehavior.Cascade));

        modelBuilder.Entity<SurveyResponse>(e =>
        {
            e.HasOne(sr => sr.Survey).WithMany(s => s.SurveyResponses).HasForeignKey(sr => sr.SurveyId);
            e.HasOne(sr => sr.Respondent).WithMany(r => r.SurveyResponses).HasForeignKey(sr => sr.RespondentId);
            e.HasOne(sr => sr.Outlet).WithMany(o => o.SurveyResponses).HasForeignKey(sr => sr.OutletId);
            e.HasOne(sr => sr.Merchandiser).WithMany(m => m.SurveyResponses).HasForeignKey(sr => sr.MerchandiserId);
        });

        modelBuilder.Entity<SurveyResponseDetail>(e =>
        {
            e.HasOne(d => d.SurveyResponse).WithMany(sr => sr.Details)
             .HasForeignKey(d => d.SurveyResponseId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(d => d.Question).WithMany(q => q.SurveyResponseDetails).HasForeignKey(d => d.QuestionId);
            e.HasOne(d => d.AnswerOption).WithMany(ao => ao.SurveyResponseDetails).HasForeignKey(d => d.AnswerOptionId);
        });
    }
}
