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
    name: 'front_environmentActions_environmentCreation',
  },
  {
    id: EnvironmentAction.EnvironmentDeletion,
    name: 'front_environmentActions_environmentDeletion',
  },
  {
    id: EnvironmentAction.EnvironmentBlockage,
    name: 'front_environmentActions_environmentBlockage',
  },
  {
    id: EnvironmentAction.TrainingRestorationSuccessed,
    name: 'front_environmentActions_trainingRestorationSuccessed',
  },
  {
    id: EnvironmentAction.TrainingRestorationFailed,
    name: 'front_environmentActions_trainingRestorationFailed',
  },
  {
    id: EnvironmentAction.TrainingLock,
    name: 'front_environmentActions_trainingLock',
  },
  {
    id: EnvironmentAction.TrainingUnlock,
    name: 'front_environmentActions_trainingUnlock',
  },
  {
    id: EnvironmentAction.TrainingPasswordChange,
    name: 'front_environmentActions_trainingPasswordChange',
  },
  {
    id: EnvironmentAction.TrainingPasswordAutoChange,
    name: 'front_environmentActions_trainingPasswordAutoChange',
  },
  {
    id: EnvironmentAction.PreviewRestorationSuccessed,
    name: 'front_environmentActions_previewRestorationSuccessed',
  },
  {
    id: EnvironmentAction.PreviewRestorationFailed,
    name: 'front_environmentActions_previewRestorationFailed',
  },
  {
    id: EnvironmentAction.PreviewLock,
    name: 'front_environmentActions_previewLock',
  },
  {
    id: EnvironmentAction.PreviewUnlock,
    name: 'front_environmentActions_previewUnlock',
  },
  {
    id: EnvironmentAction.PreviewAutomaticDeletion,
    name: 'front_environmentActions_previewAutomaticDeletion',
  },
  {
    id: EnvironmentAction.PreviewPasswordChanged,
    name: 'front_environmentActions_previewPasswordChanged',
  },
  {
    id: EnvironmentAction.ConnectAsProduction,
    name: 'front_environmentActions_connectAsProduction',
  },
  {
    id: EnvironmentAction.ConnectAsTraining,
    name: 'front_environmentActions_connectAsTraining',
  },
  {
    id: EnvironmentAction.ConnectAsPreview,
    name: 'front_environmentActions_connectAsPreview',
  },
  {
    id: EnvironmentAction.EnvironmentDeletionIntent,
    name: 'front_environmentActions_environmentDeletionIntent',
  },
  {
    id: EnvironmentAction.PreviewAutomaticRestorationSuccessed,
    name: 'front_environmentActions_previewAutomaticRestorationSuccessed',
  },
  {
    id: EnvironmentAction.PreviewAutomaticRestorationFailed,
    name: 'front_environmentActions_previewAutomaticRestorationFailed',
  },
];

