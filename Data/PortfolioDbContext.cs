using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Entities;

namespace Portfolio.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

    public DbSet<PortfolioProfileEntity> Profiles => Set<PortfolioProfileEntity>();
    public DbSet<PortfolioTechnologyEntity> Technologies => Set<PortfolioTechnologyEntity>();
    public DbSet<PortfolioSocialLinkEntity> SocialLinks => Set<PortfolioSocialLinkEntity>();
    public DbSet<PortfolioHobbyEntity> Hobbies => Set<PortfolioHobbyEntity>();
    public DbSet<PortfolioEducationEntity> Education => Set<PortfolioEducationEntity>();
    public DbSet<PortfolioExperienceEntity> Experience => Set<PortfolioExperienceEntity>();
    public DbSet<PortfolioExperienceResponsibilityEntity> ExperienceResponsibilities =>
        Set<PortfolioExperienceResponsibilityEntity>();
    public DbSet<PortfolioProjectEntity> Projects => Set<PortfolioProjectEntity>();
    public DbSet<PortfolioProjectTechnologyEntity> ProjectTechnologies => Set<PortfolioProjectTechnologyEntity>();
    public DbSet<PortfolioChallengeEntity> Challenges => Set<PortfolioChallengeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var profile = modelBuilder.Entity<PortfolioProfileEntity>();
        profile.ToTable("PortfolioProfiles");
        profile.Property(x => x.Name).HasMaxLength(200).IsRequired();
        profile.Property(x => x.Role).HasMaxLength(200).IsRequired();
        profile.Property(x => x.HeroBackgroundUrl).HasMaxLength(1024).IsRequired();
        profile.Property(x => x.AboutBioTemplatePl).HasMaxLength(4000).IsRequired();
        profile.Property(x => x.AboutBioTemplateEn).HasMaxLength(4000).IsRequired();
        profile.Property(x => x.ContactEmail).HasMaxLength(320).IsRequired();
        profile.Property(x => x.ContactPhone).HasMaxLength(64).IsRequired();

        ConfigureChildEntity(modelBuilder.Entity<PortfolioTechnologyEntity>(), "PortfolioTechnologies");
        ConfigureChildEntity(modelBuilder.Entity<PortfolioSocialLinkEntity>(), "PortfolioSocialLinks");
        ConfigureChildEntity(modelBuilder.Entity<PortfolioHobbyEntity>(), "PortfolioHobbies");
        ConfigureChildEntity(modelBuilder.Entity<PortfolioEducationEntity>(), "PortfolioEducation");
        ConfigureChildEntity(modelBuilder.Entity<PortfolioExperienceEntity>(), "PortfolioExperience");
        ConfigureChildEntity(modelBuilder.Entity<PortfolioProjectEntity>(), "PortfolioProjects");
        ConfigureChildEntity(modelBuilder.Entity<PortfolioChallengeEntity>(), "PortfolioChallenges");

        modelBuilder.Entity<PortfolioTechnologyEntity>()
            .Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<PortfolioSocialLinkEntity>()
            .Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();
        modelBuilder.Entity<PortfolioSocialLinkEntity>()
            .Property(x => x.Url)
            .HasMaxLength(2048)
            .IsRequired();

        modelBuilder.Entity<PortfolioHobbyEntity>()
            .Property(x => x.TextPl)
            .HasMaxLength(200)
            .IsRequired();
        modelBuilder.Entity<PortfolioHobbyEntity>()
            .Property(x => x.TextEn)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<PortfolioEducationEntity>()
            .Property(x => x.SchoolPl)
            .HasMaxLength(300)
            .IsRequired();
        modelBuilder.Entity<PortfolioEducationEntity>()
            .Property(x => x.SchoolEn)
            .HasMaxLength(300)
            .IsRequired();

        modelBuilder.Entity<PortfolioExperienceEntity>()
            .Property(x => x.Position)
            .HasMaxLength(200)
            .IsRequired();
        modelBuilder.Entity<PortfolioExperienceEntity>()
            .Property(x => x.Company)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<PortfolioProjectEntity>()
            .Property(x => x.Company)
            .HasMaxLength(200)
            .IsRequired();
        modelBuilder.Entity<PortfolioProjectEntity>()
            .Property(x => x.TitlePl)
            .HasMaxLength(300)
            .IsRequired();
        modelBuilder.Entity<PortfolioProjectEntity>()
            .Property(x => x.TitleEn)
            .HasMaxLength(300)
            .IsRequired();
        modelBuilder.Entity<PortfolioProjectEntity>()
            .Property(x => x.DescriptionPl)
            .HasMaxLength(4000)
            .IsRequired();
        modelBuilder.Entity<PortfolioProjectEntity>()
            .Property(x => x.DescriptionEn)
            .HasMaxLength(4000)
            .IsRequired();

        modelBuilder.Entity<PortfolioChallengeEntity>()
            .Property(x => x.TitlePl)
            .HasMaxLength(300)
            .IsRequired();
        modelBuilder.Entity<PortfolioChallengeEntity>()
            .Property(x => x.TitleEn)
            .HasMaxLength(300)
            .IsRequired();
        modelBuilder.Entity<PortfolioChallengeEntity>()
            .Property(x => x.DescriptionPl)
            .HasMaxLength(4000)
            .IsRequired();
        modelBuilder.Entity<PortfolioChallengeEntity>()
            .Property(x => x.DescriptionEn)
            .HasMaxLength(4000)
            .IsRequired();

        modelBuilder.Entity<PortfolioProjectTechnologyEntity>().ToTable("PortfolioProjectTechnologies");
        modelBuilder.Entity<PortfolioProjectTechnologyEntity>()
            .Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<PortfolioExperienceResponsibilityEntity>().ToTable("PortfolioExperienceResponsibilities");
        modelBuilder.Entity<PortfolioExperienceResponsibilityEntity>()
            .Property(x => x.TextPl)
            .HasMaxLength(600)
            .IsRequired();
        modelBuilder.Entity<PortfolioExperienceResponsibilityEntity>()
            .Property(x => x.TextEn)
            .HasMaxLength(600)
            .IsRequired();

        modelBuilder.Entity<PortfolioProjectEntity>()
            .HasMany(x => x.Technologies)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.PortfolioProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PortfolioExperienceEntity>()
            .HasMany(x => x.Responsibilities)
            .WithOne(x => x.Experience)
            .HasForeignKey(x => x.PortfolioExperienceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureChildEntity<TEntity>(EntityTypeBuilder<TEntity> entity, string tableName)
        where TEntity : class
    {
        entity.ToTable(tableName);
        entity.HasIndex("PortfolioProfileId", "SortOrder");
    }
}
