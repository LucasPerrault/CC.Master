import { ILifecycleStep } from '../models/lifecycle-step.interface';

export enum LifecycleStep {
  Referenced = 'Referenced',
  Preview = 'Preview',
  ReadyForDeploy = 'ReadyForDeploy',
  InProduction = 'InProduction',
  ToDelete = 'ToDelete',
  Deleted = 'Deleted',
}

export const getLifecycleStepName = (lifecycle: LifecycleStep): string => {
  switch (lifecycle) {
    case LifecycleStep.Referenced:
      return 'front_sourcePage_lifecycleStep_referenced';
    case LifecycleStep.Preview:
      return 'front_sourcePage_lifecycleStep_preview';
    case LifecycleStep.ReadyForDeploy:
      return 'front_sourcePage_lifecycleStep_readyForDeploy';
    case LifecycleStep.InProduction:
      return 'front_sourcePage_lifecycleStep_inProduction';
    case LifecycleStep.ToDelete:
      return 'front_sourcePage_lifecycleStep_toDelete';
    case LifecycleStep.Deleted:
      return 'front_sourcePage_lifecycleStep_deleted';
    default:
      return 'front_sourcePage_lifecycleStep_unknown';
  }
};

export const lifecycles: ILifecycleStep[] = [
  {
    id: LifecycleStep.Referenced,
    name: getLifecycleStepName(LifecycleStep.Referenced),
  },
  {
    id: LifecycleStep.Preview,
    name: getLifecycleStepName(LifecycleStep.Preview),
  },
  {
    id: LifecycleStep.ReadyForDeploy,
    name: getLifecycleStepName(LifecycleStep.ReadyForDeploy),
  },
  {
    id: LifecycleStep.InProduction,
    name: getLifecycleStepName(LifecycleStep.InProduction),
  },
  {
    id: LifecycleStep.ToDelete,
    name: getLifecycleStepName(LifecycleStep.ToDelete),
  },
  {
    id: LifecycleStep.Deleted,
    name: getLifecycleStepName(LifecycleStep.Deleted),
  },
];
