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
    translationKey: 'front_environmentActions_environmentCreation',
  },
  {
    id: EnvironmentAction.EnvironmentDeletion,
    name: 'Suppression d’un environnement',
    translationKey: 'front_environmentActions_environmentDeletion',
  },
  {
    id: EnvironmentAction.EnvironmentBlockage,
    name: 'Blocage d’un environnement',
    translationKey: 'front_environmentActions_environmentBlockage',
  },
  {
    id: EnvironmentAction.TrainingRestorationSuccessed,
    name: 'Restauration de formation en succès',
    translationKey: 'front_environmentActions_trainingRestorationSuccessed',
  },
  {
    id: EnvironmentAction.TrainingRestorationFailed,
    name: 'Restauration de formation en erreur',
    translationKey: 'front_environmentActions_trainingRestorationFailed',
  },
  {
    id: EnvironmentAction.TrainingLock,
    name: 'Protection de formation',
    translationKey: 'front_environmentActions_trainingLock',
  },
  {
    id: EnvironmentAction.TrainingUnlock,
    name: 'Déprotection de formation',
    translationKey: 'front_environmentActions_trainingUnlock',
  },
  {
    id: EnvironmentAction.TrainingPasswordChange,
    name: 'Changement du mot de passe de formation',
    translationKey: 'front_environmentActions_trainingPasswordChange',
  },
  {
    id: EnvironmentAction.TrainingPasswordAutoChange,
    name: 'Suppression automatique de formation',
    translationKey: 'front_environmentActions_trainingPasswordAutoChange',
  },
  {
    id: EnvironmentAction.PreviewRestorationSuccessed,
    name: 'Restauration de preview en succès',
    translationKey: 'front_environmentActions_previewRestorationSuccessed',
  },
  {
    id: EnvironmentAction.PreviewRestorationFailed,
    name: 'Restauration de preview en échec',
    translationKey: 'front_environmentActions_previewRestorationFailed',
  },
  {
    id: EnvironmentAction.PreviewLock,
    name: 'Protection de preview',
    translationKey: 'front_environmentActions_previewLock',
  },
  {
    id: EnvironmentAction.PreviewUnlock,
    name: 'Déprotection de preview',
    translationKey: 'front_environmentActions_previewUnlock',
  },
  {
    id: EnvironmentAction.PreviewAutomaticDeletion,
    name: 'Suppression automatique de preview',
    translationKey: 'front_environmentActions_previewAutomaticDeletion',
  },
  {
    id: EnvironmentAction.PreviewPasswordChanged,
    name: 'Changement mot de passe de preview',
    translationKey: 'front_environmentActions_previewPasswordChanged',
  },
  {
    id: EnvironmentAction.ConnectAsProduction,
    name: 'Connexion sur la production',
    translationKey: 'front_environmentActions_connectAsProduction',
  },
  {
    id: EnvironmentAction.ConnectAsTraining,
    name: 'Connexion sur la formation',
    translationKey: 'front_environmentActions_connectAsTraining',
  },
  {
    id: EnvironmentAction.ConnectAsPreview,
    name: 'Connexion sur la preview',
    translationKey: 'front_environmentActions_connectAsPreview',
  },
  {
    id: EnvironmentAction.EnvironmentDeletionIntent,
    name: 'Tentative de suppression d’un environnement',
    translationKey: 'front_environmentActions_environmentDeletionIntent',
  },
  {
    id: EnvironmentAction.PreviewAutomaticRestorationSuccessed,
    name: 'Restauration automatique de preview en succès',
    translationKey: 'front_environmentActions_previewAutomaticRestorationSuccessed',
  },
  {
    id: EnvironmentAction.PreviewAutomaticRestorationFailed,
    name: 'Restauration automatique de preview en échec',
    translationKey: 'front_environmentActions_previewAutomaticRestorationFailed',
  },
];

