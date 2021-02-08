import { IEnvironmentAction } from '@cc/domain/environments';

export enum EnvironmentAction {
  EnvironmentCreation = 1,
  EnvironmentDeletion = 2,
  EnvironmentBlockage = 3,
  TrainingRestorationSuccessed = 10,
  TrainingRestorationFailed = 11,
  TrainingLock = 12,
  TrainingUnlock = 13,
  TrainingPasswordChange = 14,
  TrainingPasswordAutoChange = 15,
  PreviewRestorationSuccessed = 20,
  PreviewRestorationFailed = 21,
  PreviewLock = 22,
  PreviewUnlock = 23,
  PreviewAutomaticDeletion = 24,
  PreviewPasswordChanged = 25,
  ConnectAsProduction = 30,
  ConnectAsTraining = 31,
  ConnectAsPreview = 32,
  EnvironmentDeletionIntent = 33,
  PreviewAutomaticRestorationSuccessed = 34,
  PreviewAutomaticRestorationFailed = 35,
}

export const environmentActions: IEnvironmentAction[] = [
  {
    id: EnvironmentAction.EnvironmentCreation,
    name: 'Création d’un environnement',
  },
  {
    id: EnvironmentAction.EnvironmentDeletion,
    name: 'Suppression d’un environnement',
  },
  {
    id: EnvironmentAction.EnvironmentBlockage,
    name: 'Blocage d’un environnement',
  },
  {
    id: EnvironmentAction.TrainingRestorationSuccessed,
    name: 'Restauration de formation en succès',
  },
  {
    id: EnvironmentAction.TrainingRestorationFailed,
    name: 'Restauration de formation en erreur',
  },
  {
    id: EnvironmentAction.TrainingLock,
    name: 'Protection de formation',
  },
  {
    id: EnvironmentAction.TrainingUnlock,
    name: 'Déprotection de formation',
  },
  {
    id: EnvironmentAction.TrainingPasswordChange,
    name: 'Changement du mot de passe de formation',
  },
  {
    id: EnvironmentAction.TrainingPasswordAutoChange,
    name: 'Suppression automatique de formation',
  },
  {
    id: EnvironmentAction.PreviewRestorationSuccessed,
    name: 'Restauration de preview en succès',
  },
  {
    id: EnvironmentAction.PreviewRestorationFailed,
    name: 'Restauration de preview en échec',
  },
  {
    id: EnvironmentAction.PreviewLock,
    name: 'Protection de preview',
  },
  {
    id: EnvironmentAction.PreviewUnlock,
    name: 'Déprotection de preview',
  },
  {
    id: EnvironmentAction.PreviewAutomaticDeletion,
    name: 'Suppression automatique de preview',
  },
  {
    id: EnvironmentAction.PreviewPasswordChanged,
    name: 'Changement mot de passe de preview',
  },
  {
    id: EnvironmentAction.ConnectAsProduction,
    name: 'Connexion sur la production',
  },
  {
    id: EnvironmentAction.ConnectAsTraining,
    name: 'Connexion sur la formation',
  },
  {
    id: EnvironmentAction.ConnectAsPreview,
    name: 'Connexion sur la preview',
  },
  {
    id: EnvironmentAction.EnvironmentDeletionIntent,
    name: 'Tentative de suppression d’un environnement',
  },
  {
    id: EnvironmentAction.PreviewAutomaticRestorationSuccessed,
    name: 'Restauration automatique de preview en succès',
  },
  {
    id: EnvironmentAction.PreviewAutomaticRestorationFailed,
    name: 'Restauration automatique de preview en échec',
  },
];

