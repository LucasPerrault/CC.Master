using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using System.Collections.Generic;

namespace Rights.Web
{
    public class OperationDescription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsBusinessItemSpecific { get; set; }
        public bool IsPrivate { get; set; }

        public List<Scope> Scopes { get; set; } = new List<Scope>();

        public static IEnumerable<OperationDescription> All()
        {
            return new List<OperationDescription>
            {
                new OperationDescription
                {
                    Id = (int)Operation.DisplayEnvTab,
                    Name = "Afficher l'onglet des environnements"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadEnvironments,
                    Name = "Voir les environnements",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly },
                    IsBusinessItemSpecific = true
                },
                new OperationDescription
                {
                    Id = (int)Operation.RestoreInstances,
                    Name = "Restaurer les instances",
                    Scopes = new List<Scope> { Scope.AllDepartments },
                    IsBusinessItemSpecific = true
                },
                new OperationDescription
                {
                    Id = (int)Operation.ImpersonationOnTrainingAndPreview,
                    Name = "Se connecter 'en tant que' sur previews et formations",
                    Scopes = new List<Scope> { Scope.AllDepartments },
                    IsBusinessItemSpecific = true
                },
                new OperationDescription
                {
                    Id = (int)Operation.AccessNonAnonymizedInstances,
                    Name = "Se connecter 'En tant que' sur les instances non anonymisées"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ImpersonationOnProd,
                    Name = "Se connecter 'en tant que' en prod",
                    Scopes = new List<Scope> { Scope.AllDepartments },
                    IsBusinessItemSpecific = true
                },
                new OperationDescription
                {
                    Id = (int)Operation.CreateInstance,
                    Name = "Créer un environnement",
                    Scopes = new List<Scope> { Scope.AllDepartments },
                    IsBusinessItemSpecific = true
                },
                new OperationDescription
                {
                    Id = (int)Operation.Desactivate,
                    Name = "Supprimer un environnement"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadDistributorRelations,
                    Name = "Voir les relations distributeurs"
                },
                new OperationDescription
                {
                    Id = (int)Operation.EnvironmentAccessesAndDistributorRelations,
                    Name = "Gérer les accès aux environnements et les relations distributeurs"
                },
                new OperationDescription
                {
                    Id = (int)Operation.Demo,
                    Name = "Créer, voir et supprimer les démos",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.TemplateDemos,
                    Name = "Se connecter sur les démos templates",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadCounts,
                    Name = "Consulter les décomptes",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartementLevel1, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadCMRR,
                    Name = "Accéder au CMRR",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.EnvironmentLogTab,
                    Name = "Voir l'onglet des logs d'environnement"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadContracts,
                    Name = "Voir les contrats",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadContractEntries,
                    Name = "Voir les lignes comptables des contrats",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.EditContractsEntities,
                    Name = "Rattacher les entités légales aux contrats",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.CloseContracts,
                    Name = "Clôturer les contrats",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.CreateContracts,
                    Name = "Créer des contrats",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.EditContracts,
                    Name = "Editer des contrats"
                },
                new OperationDescription
                {
                    Id = (int)Operation.Invoices,
                    Name = "Gérer la facturation",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.ExportClients,
                    Name = "Exporter les clients",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadDistributorsTab,
                    Name = "Voir l'onglet gestion des distributeurs",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.EditDistributors,
                    Name = "Editer les distributeurs",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadCommercialOffers,
                    Name = "Voir les offres commerciales"
                },
                new OperationDescription
                {
                    Id = (int)Operation.CreateCommercialOffers,
                    Name = "Créer et éditer les offres commerciales"
                },
                new OperationDescription
                {
                    Id = (int)Operation.CountsTrack,
                    Name = "Voir l'onglet 'suivi des décomptes'"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadFtp,
                    Name = "Voir les informations sur les FTP"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadCodeSources,
                    Name = "Voir les sources de code"
                },
                new OperationDescription
                {
                    Id = (int)Operation.EditCodeSources,
                    Name = "Editer les sources de code"
                },
                new OperationDescription
                {
                    Id = (int)Operation.EditGitHubBranchesAndPR,
                    Name = "Editer les PR et les branches GitHub"
                },
                new OperationDescription
                {
                    Id = (int)Operation.DeletePreviews,
                    Name = "Supprimer des configurations de preview"
                },
                new OperationDescription
                {
                    Id = (int)Operation.DeployAllPreviews,
                    Name = "Forcer le déploiement de toutes les configurations de preview"
                },
                new OperationDescription
                {
                    Id = (int)Operation.CreateCounts,
                    Name = "Lancer les décomptes"
                },
                new OperationDescription
                {
                    Id = (int)Operation.GeneralUpgrade,
                    Name = "Droits élevés pour le ping"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ContactRoles,
                    Name = "Voir les rôles de contact"
                },
                new OperationDescription
                {
                    Id = (int)Operation.EditClients,
                    Name = "Editer les clients"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadTenantDataRequest,
                    Name = "Voir les demandes de téléchargement de données clients",
                },
                new OperationDescription
                {
                    Id = (int)Operation.WriteTenantDataRequest,
                    Name = "Créer des demandes de téléchargement de données clients",
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadTenantDataRequestStatus,
                    Name = "Voir l'état d'avancement des téléchargements de données clients",
                },
                new OperationDescription
                {
                    Id = (int)Operation.EditEnvironmentGroup,
                    Name = "Modifier le groupe d'environnement",
                },
                new OperationDescription
                {
                    Id = (int)Operation.CloudControlInternalRequest,
                    Name = "Routes de dialogue entre l'ancien et le nouveau CC",
                },
                new OperationDescription
                {
                    Id = (int)Operation.AccessBetaFeatures,
                    Name = "Fonctionnalités beta de CC",
                },
                new OperationDescription
                {
                    Id = (int)Operation.SyncAllCafe,
                    Name = "Synchroniser Café",
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadAllCafe,
                    Name = "Voir Café",
                },
                new OperationDescription
                {
                    Id = (int)Operation.HangfireNetcoreRequest,
                    Name = "(WS ONLY) Déclencher les routes Hangfire",
                },
                new OperationDescription
                {
                    Id = (int)Operation.UpdatePreviewDeployStatus,
                    Name = "(WS ONLY) Mettre à jour les états des déploiements",
                    IsPrivate = true
                },
                new OperationDescription
                {
                    Id = (int)Operation.SalesforceSync,
                    Name = "(WS ONLY) Lancer une synchronisation avec Salesforce",
                    IsPrivate = true
                },
                new OperationDescription
                {
                    Id = (int)Operation.DeleteInstance,
                    Name = "(WS ONLY) Supprimer une instance",
                    IsPrivate = true
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadInstances,
                    Name = "[Legacy] Voir les environnements",
                    Scopes = new List<Scope> { Scope.AllDepartments, Scope.DepartmentOnly }
                },
                new OperationDescription
                {
                    Id = (int)Operation.UserImpersonationPreviewAndTraining,
                    Name = "[Legacy] Connecter 'En tant que' sur les previews et formations"
                },
                new OperationDescription
                {
                    Id = (int)Operation.UserImpersonationProd,
                    Name = "[Legacy] Connecter 'En tant que' en production"
                },
                new OperationDescription
                {
                    Id = (int)Operation.UserProdImpersonationForRestrictedEnvironments,
                    Name = "[legacy] Se connecter 'en tant que' sur la prod d'une instance technique protégée"
                },
                new OperationDescription
                {
                    Id = (int)Operation.ReadLuccaInstances,
                    Name = "[Legacy] Voir les environnements Lucca et Partenaires"
                },
                new OperationDescription
                {
                    Id = (int)Operation.Restore,
                    Name = "[Legacy] Restaurer les previews et formations"
                },
                new OperationDescription
                {
                    Id = (int)Operation.RestoreRestrictedEnvironments,
                    Name = "[Legacy] Restaurer les instances d'environnement protégés en restauration"
                },
                new OperationDescription
                {
                    Id = (int)Operation.Preview,
                    Name = "[Legacy] Previews"
                },
                new OperationDescription
                {
                    Id = (int)Operation.UserActifDeprecated,
                    Name = "[Legacy] Voir l'onglet 'Utilisateurs actifs'"
                },
                new OperationDescription
                {
                    Id = (int)Operation.Counts,
                    Name = "[Legacy] Décomptes"
                },
                new OperationDescription
                {
                    Id = (int)Operation.Accounting,
                    Name = "[Legacy] Comptabilisation"
                }
            };
        }

        public class CcScope
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
