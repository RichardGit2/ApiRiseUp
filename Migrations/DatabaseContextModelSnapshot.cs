using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RiseUpAPI.Data;

namespace RiseUpAPI.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20240312000000_AddNewFieldsAndAudience")]
public partial class DatabaseContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("RiseUpAPI.Models.Activity", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer");

            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

            b.Property<string>("Category")
                .HasColumnType("text");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("text");

            b.Property<int>("OpportunityId")
                .HasColumnType("integer");

            b.HasKey("Id");

            b.HasIndex("OpportunityId");

            b.ToTable("Activities");
        });

        modelBuilder.Entity("RiseUpAPI.Models.Audience", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer");

            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

            b.Property<int>("OpportunityId")
                .HasColumnType("integer");

            b.Property<string[]>("Regions")
                .HasColumnType("text[]");

            b.Property<string>("Scope")
                .IsRequired()
                .HasColumnType("text");

            b.HasKey("Id");

            b.HasIndex("OpportunityId")
                .IsUnique();

            b.ToTable("Audiences");
        });

        modelBuilder.Entity("RiseUpAPI.Models.Opportunity", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer");

            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

            b.Property<string>("Description")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("Duration")
                .HasColumnType("text");

            b.Property<int>("OrganizationId")
                .HasColumnType("integer");

            b.Property<bool>("RemoteOrOnline")
                .HasColumnType("boolean");

            b.Property<string>("Title")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("Url")
                .HasColumnType("text");

            b.HasKey("Id");

            b.HasIndex("OrganizationId");

            b.ToTable("Opportunities");
        });

        modelBuilder.Entity("RiseUpAPI.Models.Organization", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer");

            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

            b.Property<string>("Logo")
                .HasColumnType("text");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("Url")
                .HasColumnType("text");

            b.HasKey("Id");

            b.ToTable("Organizations");
        });

        modelBuilder.Entity("RiseUpAPI.Models.Activity", b =>
        {
            b.HasOne("RiseUpAPI.Models.Opportunity", "Opportunity")
                .WithMany("Activities")
                .HasForeignKey("OpportunityId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("RiseUpAPI.Models.Audience", b =>
        {
            b.HasOne("RiseUpAPI.Models.Opportunity", "Opportunity")
                .WithOne("Audience")
                .HasForeignKey("RiseUpAPI.Models.Audience", "OpportunityId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("RiseUpAPI.Models.Opportunity", b =>
        {
            b.HasOne("RiseUpAPI.Models.Organization", "Organization")
                .WithMany("Opportunities")
                .HasForeignKey("OrganizationId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
    }
} 