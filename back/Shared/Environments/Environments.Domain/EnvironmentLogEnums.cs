using System;
using System.ComponentModel;

namespace Environments.Domain
{
    public enum EnvironmentLogActivity
    {
        [field: Description("Création environnement")]
        EnvironmentCreation = 1,
        [field: Description("Destruction environnement")]
        EnvironmentDeletion = 2,
        [field: Description("Blocage environnement")]
        EnvironmentbBlockage = 3,
        [field: Description("Restauration formation en succès")]
        TrainingRestorationSucceeded = 10,
        [field: Description("Restauration formation en erreur")]
        TrainingRestorationFailed = 11,
        [field: Description("Verrouillage formation")]
        TrainingLock = 12,
        [field: Description("Déverrouillage formation")]
        TrainingUnlock = 13,
        [field: Description("Changement mot de passe de formation")]
        TrainingPasswordChanged = 14,
        [field: Description("Suppression automatique de la formation")]
        TrainingAutomaticDeletion = 15,
        [field: Description("Restauration preview en succès")]
        PreviewRestorationSuccessed = 20,
        [field: Description("Restauration preview en erreur")]
        PreviewRestorationFailed = 21,
        [field: Description("Verrouillage preview")]
        PreviewLock = 22,
        [field: Description("Déverrouillage preview")]
        PreviewUnlock = 23,
        [field: Description("Suppression automatique de la preview")]
        PreviewAutomaticDeletion = 24,
        [field: Description("Changement mot de passe de preview")]
        PreviewPasswordChanged = 25,
        [field: Description("Connexion 'en tant que' à l'instance de production")]
        ConnectAsProduction = 30,
        [field: Description("Connexion 'en tant que' à l'instance de formation")]
        ConnectAsTraining = 31,
        [field: Description("Connexion 'en tant que' à l'instance de preview")]
        ConnectAsPreview = 32,
        [field: Description("Tentative de suppression d'environnement")]
        EnvironmentDeletionIntent = 33,
        [field: Description("Restauration preview automatique en succès")]
        PreviewAutomaticRestorationSuccessed = 34,
        [field: Description("Restauration preview automatique en erreur")]
        PreviewAutomaticRestorationFailed = 35,
    }

    public enum EnvironmentLogMessageTypes
    {
        INTERNAL = 0,
        USER = 1,
        EXPLANATION = 2
    }

}
