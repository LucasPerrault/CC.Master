using BoDi;
using Distributors.Domain.Models;
using Environments.Domain;
using Environments.Infra.Storage;
using Instances.Application.Specflow.Tests.Trainings.Models;
using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Trainings;
using Instances.Infra.Storage;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Testing.Infra;
using Testing.Specflow;

using CcEnvironment = Environments.Domain.Environment;

namespace Instances.Application.Specflow.Tests.Trainings.Steps
{
    [Binding]
    public class StartupStep
    {
        private readonly IObjectContainer _objectContainer;

        public StartupStep(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario("training")]
        public void InitializeScenario()
        {
            var context = new SpecflowTestContext();
            var instancesDbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));
            var environmentsDbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Environments", o => new EnvironmentsDbContext(o));
            _objectContainer.RegisterInstanceAs(context);
            _objectContainer.RegisterInstanceAs(instancesDbContext);
            _objectContainer.RegisterInstanceAs(environmentsDbContext);

            var trainingRestorationTestResults = new TrainingRestorationTestResults();
            _objectContainer.RegisterInstanceAs(trainingRestorationTestResults);
            var trainingRestorationEndTestResults = new TrainingRestorationEndTestResults();
            _objectContainer.RegisterInstanceAs(trainingRestorationEndTestResults);

            var luccaDistributor = new Distributor
            {
                Id = 1,
                Code = "LUCCA",
                Name = "Lucca",
            };

            var otherDistributor = new Distributor()
            {
                Id = 2,
                Code = "DISTRIBUTOR",
                Name = "Other distributor",
            };

            context.Distributors.Add(luccaDistributor);
            context.Distributors.Add(otherDistributor);
            instancesDbContext.AddRange(context.Distributors);

            var noExistingTrainingEnvironment = new CcEnvironment
            {
                Id = 1,
                IsActive = true,
                Cluster = "CLUSTER5",
                CreatedAt = System.DateTime.Now,
                Domain = EnvironmentDomain.ILuccaDotNet,
                Purpose = EnvironmentPurpose.Contractual,
                Subdomain = "no-existing-training",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Access = new EnvironmentAccess
                        {
                            DistributorId = luccaDistributor.Id,
                            Type = EnvironmentAccessTypeEnum.Contract,
                            StartsAt = System.DateTime.Now,
                        }
                    }
                }
            };
            instancesDbContext.Add(noExistingTrainingEnvironment);
            environmentsDbContext.Add(noExistingTrainingEnvironment);

            var luccaOnlyTrainingEnvironment = new CcEnvironment
            {
                Id = 2,
                IsActive = true,
                Cluster = "CLUSTER5",
                CreatedAt = System.DateTime.Now,
                Domain = EnvironmentDomain.ILuccaDotNet,
                Purpose = EnvironmentPurpose.Contractual,
                Subdomain = "lucca-only-environment",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Access = new EnvironmentAccess
                        {
                            DistributorId = luccaDistributor.Id,
                            Type = EnvironmentAccessTypeEnum.Contract,
                            StartsAt = System.DateTime.Now,
                        }
                    }
                }
            };

            instancesDbContext.Add(luccaOnlyTrainingEnvironment);
            environmentsDbContext.Add(luccaOnlyTrainingEnvironment);

            var withPreviousActiveTrainingEnvironment = new CcEnvironment
            {
                Id = 3,
                IsActive = true,
                Cluster = "CLUSTER5",
                CreatedAt = System.DateTime.Now,
                Domain = EnvironmentDomain.ILuccaDotNet,
                Purpose = EnvironmentPurpose.Contractual,
                Subdomain = "with-previous-active-training",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Access = new EnvironmentAccess
                        {
                            DistributorId = luccaDistributor.Id,
                            Type = EnvironmentAccessTypeEnum.Contract,
                            StartsAt = System.DateTime.Now,
                        }
                    }
                }
            };

            instancesDbContext.Add(withPreviousActiveTrainingEnvironment);
            environmentsDbContext.Add(withPreviousActiveTrainingEnvironment);

            var noActiveTrainingEnvironment = new CcEnvironment
            {
                Id = 4,
                IsActive = true,
                Cluster = "CLUSTER5",
                CreatedAt = System.DateTime.Now,
                Domain = EnvironmentDomain.ILuccaDotNet,
                Purpose = EnvironmentPurpose.Contractual,
                Subdomain = "with-no-active-training",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Access = new EnvironmentAccess
                        {
                            DistributorId = luccaDistributor.Id,
                            Type = EnvironmentAccessTypeEnum.Contract,
                            StartsAt = System.DateTime.Now,
                        }
                    }
                }
            };

            instancesDbContext.Add(noActiveTrainingEnvironment);
            environmentsDbContext.Add(noActiveTrainingEnvironment);

            var previousTrainingInstance = new Instance
            {
                Id = 1,
                EnvironmentId = withPreviousActiveTrainingEnvironment.Id,
                IsActive = true,
                Type = InstanceType.Training,
                IsProtected = false,
                IsAnonymized = false,
                AllUsersImposedPassword = null,
            };

            instancesDbContext.Add(previousTrainingInstance);

            var inactiveTrainingInstance = new Instance
            {
                Id = 2,
                EnvironmentId = noActiveTrainingEnvironment.Id,
                IsActive = false,
                Type = InstanceType.Training,
                IsProtected = false,
                IsAnonymized = false,
                AllUsersImposedPassword = null,
            };

            instancesDbContext.Add(inactiveTrainingInstance);

            var figgoCleanCommonUri = new Uri("http://127.0.0.1/figgo/clean.common.sql");
            var figgoCleanTrainingUri = new Uri("http://127.0.0.1/figgo/clean.training.sql");
            trainingRestorationTestResults.TrainingCleaningScriptsUri.Add(figgoCleanCommonUri);
            trainingRestorationTestResults.TrainingCleaningScriptsUri.Add(figgoCleanTrainingUri);

            var figgoAnonUri = new Uri("http://127.0.0.1/figgo/anon.sql");
            trainingRestorationTestResults.AnonymizationScriptsUri.Add(figgoAnonUri);

            var monolithCleanCommonUri = new Uri("http://127.0.0.1/monolith/clean.common.sql");
            var monolithCleanTrainingUri = new Uri("http://127.0.0.1/monolith/clean.training.sql");
            trainingRestorationTestResults.TrainingCleaningScriptsUri.Add(monolithCleanCommonUri);
            trainingRestorationTestResults.TrainingCleaningScriptsUri.Add(monolithCleanTrainingUri);

            var monolithAnonUri = new Uri("http://127.0.0.1/monolith/anon.sql");
            trainingRestorationTestResults.AnonymizationScriptsUri.Add(monolithAnonUri);

            var codeSources = new List<CodeSource>{
                new CodeSource
                {
                    Code = "figgo",
                    Id = 1,
                    Type = CodeSourceType.App,
                    Lifecycle = CodeSourceLifecycleStep.InProduction,
                    RepoId = 1,
                    Repo = new GithubRepo
                    {
                        Id = 1,
                        Name = "Figgo",
                    },
                    CodeSourceArtifacts = new List<CodeSourceArtifacts>
                    {
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.CleanScript,
                            ArtifactUrl = figgoCleanCommonUri,
                            CodeSourceId = 1,
                            FileName = "clean.common.sql",
                            Id = 10,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.CleanScript,
                            ArtifactUrl = figgoCleanTrainingUri,
                            CodeSourceId = 1,
                            FileName = "clean.training.sql",
                            Id = 11,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.CleanScript,
                            ArtifactUrl = new Uri("http://127.0.0.1/figgo/clean.preview.sql"),
                            CodeSourceId = 1,
                            FileName = "clean.preview.sql",
                            Id = 15,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.AnonymizationScript,
                            ArtifactUrl = figgoAnonUri,
                            CodeSourceId = 1,
                            FileName = "anon.sql",
                            Id = 12,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.BackZip,
                            ArtifactUrl = new Uri("http://127.0.0.1/figgo/back.zip"),
                            CodeSourceId = 1,
                            FileName = "back.zip",
                            Id = 13,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.FrontZip,
                            ArtifactUrl = new Uri("http://127.0.0.1/figgo/front.zip"),
                            CodeSourceId = 1,
                            FileName = "front.zip",
                            Id = 14,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.Other,
                            ArtifactUrl = new Uri("http://127.0.0.1/figgo/random.sql"),
                            CodeSourceId = 1,
                            FileName = "random.sql",
                            Id = 16,
                        },
                    }
                },
                new CodeSource
                {
                    Code = "ilucca",
                    Id = 2,
                    Type = CodeSourceType.Monolithe,
                    Lifecycle = CodeSourceLifecycleStep.InProduction,
                    RepoId = 2,
                    Repo = new GithubRepo
                    {
                        Id = 2,
                        Name = "ilucca",
                    },
                    CodeSourceArtifacts = new List<CodeSourceArtifacts>
                    {
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.CleanScript,
                            ArtifactUrl = monolithCleanCommonUri,
                            CodeSourceId = 2,
                            FileName = "clean.common.sql",
                            Id = 20,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.CleanScript,
                            ArtifactUrl = monolithCleanTrainingUri,
                            CodeSourceId = 2,
                            FileName = "clean.training.sql",
                            Id = 21,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.CleanScript,
                            ArtifactUrl = new Uri("http://127.0.0.1/monolith/clean.preview.sql"),
                            CodeSourceId = 2,
                            FileName = "clean.preview.sql",
                            Id = 25,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.AnonymizationScript,
                            ArtifactUrl = monolithAnonUri,
                            CodeSourceId = 2,
                            FileName = "anon.sql",
                            Id = 22,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.BackZip,
                            ArtifactUrl = new Uri("http://127.0.0.1/monolith/back.zip"),
                            CodeSourceId = 2,
                            FileName = "back.zip",
                            Id = 23,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.FrontZip,
                            ArtifactUrl = new Uri("http://127.0.0.1/monolith/front.zip"),
                            CodeSourceId = 2,
                            FileName = "front.zip",
                            Id = 24,
                        },
                    }
                },
                new CodeSource
                {
                    Code = "ws",
                    Id = 3,
                    Type = CodeSourceType.WebService,
                    Lifecycle = CodeSourceLifecycleStep.InProduction,
                    RepoId = 3,
                    Repo = new GithubRepo
                    {
                        Id = 3,
                        Name = "ws",
                    },
                    CodeSourceArtifacts = new List<CodeSourceArtifacts>
                    {
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.BackZip,
                            ArtifactUrl = new Uri("http://127.0.0.1/ws/back.zip"),
                            CodeSourceId = 2,
                            FileName = "back.zip",
                            Id = 31,
                        },
                        new CodeSourceArtifacts
                        {
                            ArtifactType = CodeSourceArtifactType.FrontZip,
                            ArtifactUrl = new Uri("http://127.0.0.1/ws/front.zip"),
                            CodeSourceId = 3,
                            FileName = "front.zip",
                            Id = 32,
                        },
                    }
                },
            };
            instancesDbContext.AddRange(codeSources);

            var finishedWithFailureInstanceDuplicationId = Guid.NewGuid();
            instancesDbContext.Add(new TrainingRestoration
            {
                Anonymize = false,
                ApiKeyStorableId = null,
                AuthorId = 0,
                Comment = "",
                CommentExpiryDate = null,
                KeepExistingTrainingPasswords = true,
                RestoreFiles = false,
                Id = 1,
                EnvironmentId = luccaOnlyTrainingEnvironment.Id,
                InstanceDuplicationId = finishedWithFailureInstanceDuplicationId,
                InstanceDuplication = new InstanceDuplication
                {
                    Id= finishedWithFailureInstanceDuplicationId,
                    DistributorId = luccaDistributor.Id,
                    EndedAt = DateTime.Now.AddHours(-10).AddSeconds(1),
                    Progress = InstanceDuplicationProgress.FinishedWithFailure,
                    SourceCluster = luccaOnlyTrainingEnvironment.Cluster,
                    TargetCluster = "TEST",
                    SourceSubdomain = luccaOnlyTrainingEnvironment.Subdomain,
                    SourceType = InstanceType.Prod,
                    TargetType = InstanceType.Training,
                    TargetSubdomain = luccaOnlyTrainingEnvironment.Subdomain,
                },
            });

            var finishedWithSuccessInstanceDuplicationId = Guid.NewGuid();
            instancesDbContext.Add(new TrainingRestoration
            {
                Anonymize = false,
                ApiKeyStorableId = null,
                AuthorId = 0,
                Comment = "",
                CommentExpiryDate = null,
                KeepExistingTrainingPasswords = true,
                RestoreFiles = false,
                Id = 2,
                EnvironmentId = luccaOnlyTrainingEnvironment.Id,
                InstanceDuplicationId = finishedWithSuccessInstanceDuplicationId,
                InstanceDuplication = new InstanceDuplication
                {
                    Id = finishedWithSuccessInstanceDuplicationId,
                    DistributorId = luccaDistributor.Id,
                    EndedAt = DateTime.Now.AddHours(-9).AddSeconds(1),
                    Progress = InstanceDuplicationProgress.FinishedWithSuccess,
                    SourceCluster = luccaOnlyTrainingEnvironment.Cluster,
                    TargetCluster = "TEST",
                    SourceSubdomain = luccaOnlyTrainingEnvironment.Subdomain,
                    SourceType = InstanceType.Prod,
                    TargetType = InstanceType.Training,
                    TargetSubdomain = luccaOnlyTrainingEnvironment.Subdomain,
                },
            });

            var runningInstanceDuplicationId = Guid.NewGuid();
            instancesDbContext.Add(new TrainingRestoration
            {
                Anonymize = false,
                ApiKeyStorableId = null,
                AuthorId = 0,
                Comment = "",
                CommentExpiryDate = null,
                KeepExistingTrainingPasswords = true,
                RestoreFiles = false,
                Id = 3,
                EnvironmentId = luccaOnlyTrainingEnvironment.Id,
                InstanceDuplicationId = runningInstanceDuplicationId,
                InstanceDuplication = new InstanceDuplication
                {
                    Id = runningInstanceDuplicationId,
                    DistributorId = luccaDistributor.Id,
                    EndedAt = null,
                    Progress = InstanceDuplicationProgress.Running,
                    SourceCluster = luccaOnlyTrainingEnvironment.Cluster,
                    TargetCluster = "TEST",
                    SourceSubdomain = luccaOnlyTrainingEnvironment.Subdomain,
                    SourceType = InstanceType.Prod,
                    TargetType = InstanceType.Training,
                    TargetSubdomain = luccaOnlyTrainingEnvironment.Subdomain,
                },
            });

            var previousTrainingInstanceDuplicationId = Guid.NewGuid();
            var previousTrainingRestoration = new TrainingRestoration
            {
                Anonymize = false,
                ApiKeyStorableId = null,
                AuthorId = 0,
                Comment = "",
                CommentExpiryDate = null,
                KeepExistingTrainingPasswords = true,
                RestoreFiles = false,
                Id = 4,
                EnvironmentId = withPreviousActiveTrainingEnvironment.Id,
                InstanceDuplicationId = previousTrainingInstanceDuplicationId,
                InstanceDuplication = new InstanceDuplication
                {
                    Id = previousTrainingInstanceDuplicationId,
                    DistributorId = luccaDistributor.Id,
                    EndedAt = DateTime.Now.AddHours(-8).AddSeconds(1),
                    Progress = InstanceDuplicationProgress.FinishedWithSuccess,
                    SourceCluster = withPreviousActiveTrainingEnvironment.Cluster,
                    TargetCluster = "TEST",
                    SourceSubdomain = withPreviousActiveTrainingEnvironment.Subdomain,
                    SourceType = InstanceType.Prod,
                    TargetType = InstanceType.Training,
                    TargetSubdomain = withPreviousActiveTrainingEnvironment.Subdomain,
                },
            };
            instancesDbContext.Add(previousTrainingRestoration);

            instancesDbContext.Add(new Training
            {
                ApiKeyStorableId = null,
                AuthorId = 1,
                EnvironmentId = withPreviousActiveTrainingEnvironment.Id,
                Id = 1,
                InstanceId = previousTrainingInstance.Id,
                IsActive = true,
                LastRestoredAt = DateTime.Now,
                TrainingRestorationId = previousTrainingRestoration.Id,
            });



            instancesDbContext.SaveChanges();
            environmentsDbContext.SaveChanges();
        }

    }
}
