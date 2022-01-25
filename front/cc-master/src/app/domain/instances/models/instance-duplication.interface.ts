import { InstanceType } from '../constants/instance-type.enum';

export enum InstanceDuplicationProgress {
  Pending = 'Pending',
  Running = 'Running',
  FinishedWithSuccess = 'FinishedWithSuccess',
  FinishedWithFailure = 'FinishedWithFailure',
  Canceled = 'Canceled',
}

export interface IInstanceDuplication {
  id: string;
  progress: InstanceDuplicationProgress;
  targetCluster: string;
  targetSubdomain: string;
  targetType: InstanceType;
  sourceCluster: string;
  sourceSubdomain: string;
  sourceType: InstanceType;
  distributorId: number;
  endedAt: string;
  startedAt: string;
}
