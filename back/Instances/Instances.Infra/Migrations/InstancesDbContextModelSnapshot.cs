﻿// <auto-generated />
using System;
using Instances.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Instances.Infra.Migrations
{
    [DbContext(typeof(InstancesDbContext))]
    partial class InstancesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("instances")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Distributors.Domain.Models.Distributor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<bool>("IsAllowingCommercialCommunication")
                        .HasColumnType("bit")
                        .HasColumnName("IsAllowingCommercialCommunication");

                    b.Property<bool>("IsLucca")
                        .HasColumnType("bit")
                        .HasColumnName("IsLucca");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("Distributors", "shared");
                });

            modelBuilder.Entity("GithubBranchesCodeSources", b =>
                {
                    b.Property<int>("codeSourceId")
                        .HasColumnType("int");

                    b.Property<int>("githubBranchId")
                        .HasColumnType("int");

                    b.HasKey("codeSourceId", "githubBranchId");

                    b.HasIndex("githubBranchId");

                    b.ToTable("GithubBranchesCodeSources", "instances");
                });

            modelBuilder.Entity("GithubPullRequestsCodeSources", b =>
                {
                    b.Property<int>("codeSourceId")
                        .HasColumnType("int");

                    b.Property<int>("githubPullRequestId")
                        .HasColumnType("int");

                    b.HasKey("codeSourceId", "githubPullRequestId");

                    b.HasIndex("githubPullRequestId");

                    b.ToTable("GithubPullRequestsCodeSources", "instances");
                });

            modelBuilder.Entity("Instances.Domain.CodeSources.CodeSource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<string>("GithubRepo")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("GithubRepo");

                    b.Property<string>("JenkinsProjectName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("JenkinsProjectName");

                    b.Property<string>("JenkinsProjectUrl")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("JenkinsProjectUrl");

                    b.Property<int>("Lifecycle")
                        .HasColumnType("int")
                        .HasColumnName("Lifecycle");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<int>("Type")
                        .HasColumnType("int")
                        .HasColumnName("Type");

                    b.HasKey("Id");

                    b.ToTable("CodeSources", "instances");
                });

            modelBuilder.Entity("Instances.Domain.CodeSources.CodeSourceArtifacts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ArtifactType")
                        .HasColumnType("int")
                        .HasColumnName("ArtifactType");

                    b.Property<string>("ArtifactUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ArtifactUrl");

                    b.Property<int>("CodeSourceId")
                        .HasColumnType("int")
                        .HasColumnName("CodeSourceId");

                    b.Property<string>("FileName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("FileName");

                    b.HasKey("Id");

                    b.HasIndex("CodeSourceId");

                    b.ToTable("CodeSourceArtifacts", "instances");
                });

            modelBuilder.Entity("Instances.Domain.CodeSources.CodeSourceConfig", b =>
                {
                    b.Property<int>("CodeSourceId")
                        .HasColumnType("int");

                    b.Property<string>("AppPath")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("AppPath");

                    b.Property<string>("IisServerPath")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("IisServerPath");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit")
                        .HasColumnName("IsPrivate");

                    b.Property<string>("Subdomain")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Subdomain");

                    b.HasKey("CodeSourceId");

                    b.ToTable("CodeSourceConfigs", "instances");
                });

            modelBuilder.Entity("Instances.Domain.CodeSources.CodeSourceProductionVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BranchName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("BranchName");

                    b.Property<int>("CodeSourceId")
                        .HasColumnType("int")
                        .HasColumnName("CodeSourceId");

                    b.Property<string>("CommitHash")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CommitHash");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2")
                        .HasColumnName("Date");

                    b.Property<int>("JenkinsBuildNumber")
                        .HasColumnType("int")
                        .HasColumnName("JenkinsBuildNumber");

                    b.HasKey("Id");

                    b.HasIndex("CodeSourceId");

                    b.ToTable("CodeSourcesProductionVersions", "instances");
                });

            modelBuilder.Entity("Instances.Domain.Demos.Demo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AuthorId")
                        .HasColumnType("int")
                        .HasColumnName("authorId");

                    b.Property<string>("Cluster")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Cluster");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("comment");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("createdAt");

                    b.Property<DateTime>("DeletionScheduledOn")
                        .HasColumnType("datetime2")
                        .HasColumnName("deletionScheduledOn");

                    b.Property<int>("DistributorId")
                        .HasColumnType("int")
                        .HasColumnName("distributorID");

                    b.Property<int>("InstanceID")
                        .HasColumnType("int")
                        .HasColumnName("instanceId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("isActive");

                    b.Property<bool>("IsTemplate")
                        .HasColumnType("bit");

                    b.Property<string>("Subdomain")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("subdomain");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("DistributorId");

                    b.HasIndex("InstanceID")
                        .IsUnique();

                    b.ToTable("Demos", "instances");
                });

            modelBuilder.Entity("Instances.Domain.Demos.DemoDuplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AuthorId")
                        .HasColumnType("int")
                        .HasColumnName("authorId");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("comment");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("createdAt");

                    b.Property<Guid>("InstanceDuplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("password");

                    b.Property<int>("SourceDemoId")
                        .HasColumnType("int")
                        .HasColumnName("sourceDemoId");

                    b.HasKey("Id");

                    b.HasIndex("InstanceDuplicationId");

                    b.ToTable("DemoDuplications", "instances");
                });

            modelBuilder.Entity("Instances.Domain.Github.Models.GithubBranch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("createdAt");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("deletedAt");

                    b.Property<string>("HeadCommitHash")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("headCommitHash");

                    b.Property<string>("HeadCommitMessage")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("headCommitMessage");

                    b.Property<string>("HelmChart")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("helmChart");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("isDeleted");

                    b.Property<DateTime?>("LastPushedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("lastPushedAt");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("GithubBranches", "instances");
                });

            modelBuilder.Entity("Instances.Domain.Github.Models.GithubPullRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("ClosedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("closedAt");

                    b.Property<bool>("IsOpened")
                        .HasColumnType("bit")
                        .HasColumnName("isOpened");

                    b.Property<DateTime?>("MergedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("mergedAt");

                    b.Property<int>("Number")
                        .HasColumnType("int")
                        .HasColumnName("number");

                    b.Property<DateTime>("OpenedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("openedAt");

                    b.Property<int>("OriginBranchId")
                        .HasColumnType("int")
                        .HasColumnName("originBranchId");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("title");

                    b.HasKey("Id");

                    b.HasIndex("OriginBranchId");

                    b.ToTable("GithubPullRequests", "instances");
                });

            modelBuilder.Entity("Instances.Domain.Instances.InstanceDuplication", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DistributorId")
                        .HasColumnType("int")
                        .HasColumnName("distributorId");

                    b.Property<DateTime?>("EndedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("endedAt");

                    b.Property<int>("Progress")
                        .HasColumnType("int")
                        .HasColumnName("progress");

                    b.Property<string>("SourceCluster")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("sourceCluster");

                    b.Property<string>("SourceSubdomain")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("sourceSubdomain");

                    b.Property<int>("SourceType")
                        .HasColumnType("int")
                        .HasColumnName("sourceType");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("startedAt");

                    b.Property<string>("TargetCluster")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("targetCluster");

                    b.Property<string>("TargetSubdomain")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("targetSubdomain");

                    b.Property<int>("TargetType")
                        .HasColumnType("int")
                        .HasColumnName("targetType");

                    b.HasKey("Id");

                    b.HasIndex("DistributorId");

                    b.ToTable("InstanceDuplications", "instances");
                });

            modelBuilder.Entity("Instances.Domain.Instances.Models.Instance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AllUsersImposedPassword")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("AllUsersImposedPassword");

                    b.Property<int?>("EnvironmentId")
                        .HasColumnType("int")
                        .HasColumnName("EnvironmentId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<bool>("IsAnonymized")
                        .HasColumnType("bit")
                        .HasColumnName("IsAnonymized");

                    b.Property<bool>("IsProtected")
                        .HasColumnType("bit")
                        .HasColumnName("IsProtected");

                    b.Property<int>("Type")
                        .HasColumnType("int")
                        .HasColumnName("Type");

                    b.HasKey("Id");

                    b.ToTable("Instances", "instances");
                });

            modelBuilder.Entity("Users.Domain.SimpleUser", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("PartenairesId");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentId");

                    b.Property<int>("DistributorId")
                        .HasColumnType("int")
                        .HasColumnName("DistributorId");

                    b.Property<string>("FirstName")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("FirstName");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<string>("LastName")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("LastName");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Users", "shared");
                });

            modelBuilder.Entity("GithubBranchesCodeSources", b =>
                {
                    b.HasOne("Instances.Domain.CodeSources.CodeSource", null)
                        .WithMany()
                        .HasForeignKey("codeSourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Instances.Domain.Github.Models.GithubBranch", null)
                        .WithMany()
                        .HasForeignKey("githubBranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GithubPullRequestsCodeSources", b =>
                {
                    b.HasOne("Instances.Domain.CodeSources.CodeSource", null)
                        .WithMany()
                        .HasForeignKey("codeSourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Instances.Domain.Github.Models.GithubPullRequest", null)
                        .WithMany()
                        .HasForeignKey("githubPullRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Instances.Domain.CodeSources.CodeSourceArtifacts", b =>
                {
                    b.HasOne("Instances.Domain.CodeSources.CodeSource", null)
                        .WithMany("CodeSourceArtifacts")
                        .HasForeignKey("CodeSourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Instances.Domain.CodeSources.CodeSourceConfig", b =>
                {
                    b.HasOne("Instances.Domain.CodeSources.CodeSource", null)
                        .WithOne("Config")
                        .HasForeignKey("Instances.Domain.CodeSources.CodeSourceConfig", "CodeSourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Instances.Domain.CodeSources.CodeSourceProductionVersion", b =>
                {
                    b.HasOne("Instances.Domain.CodeSources.CodeSource", null)
                        .WithMany("ProductionVersions")
                        .HasForeignKey("CodeSourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Instances.Domain.Demos.Demo", b =>
                {
                    b.HasOne("Users.Domain.SimpleUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Distributors.Domain.Models.Distributor", "Distributor")
                        .WithMany()
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Instances.Domain.Instances.Models.Instance", "Instance")
                        .WithOne()
                        .HasForeignKey("Instances.Domain.Demos.Demo", "InstanceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Distributor");

                    b.Navigation("Instance");
                });

            modelBuilder.Entity("Instances.Domain.Demos.DemoDuplication", b =>
                {
                    b.HasOne("Instances.Domain.Instances.InstanceDuplication", "InstanceDuplication")
                        .WithMany()
                        .HasForeignKey("InstanceDuplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InstanceDuplication");
                });

            modelBuilder.Entity("Instances.Domain.Github.Models.GithubPullRequest", b =>
                {
                    b.HasOne("Instances.Domain.Github.Models.GithubBranch", "OriginBranch")
                        .WithMany()
                        .HasForeignKey("OriginBranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OriginBranch");
                });

            modelBuilder.Entity("Instances.Domain.Instances.InstanceDuplication", b =>
                {
                    b.HasOne("Distributors.Domain.Models.Distributor", "Distributor")
                        .WithMany()
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Distributor");
                });

            modelBuilder.Entity("Instances.Domain.CodeSources.CodeSource", b =>
                {
                    b.Navigation("CodeSourceArtifacts");

                    b.Navigation("Config");

                    b.Navigation("ProductionVersions");
                });
#pragma warning restore 612, 618
        }
    }
}
