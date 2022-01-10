import { IContractEstablishment } from './contract-establishment.interface';
import { IEstablishmentAttachment } from './establishment-attachment.interface';

export enum ListEntryType {
  Error = 'Error',
  Excluded = 'Exclude',
  LinkedToThisContract = 'LinkedToThisContract',
  LinkedToAnotherContract = 'LinkedToAnotherContract',
}

export enum LifecycleStep {
  StartInTheFuture = 'StartInTheFuture',
  InProgress = 'InProgress',
  Finished = 'Finished',
  Inactive = 'Inactive',
  WaitingFirstActivation = 'WaitingFirstActivation',
  NotLinkedSince = 'NotLinkedSince',
  NotLinkedBetweenPastAndFuture = 'NotLinkedBetweenPastAndFuture',
  Excluded = 'Excluded',
  Unknown = 'Unknown',
}

export interface IListEntry {
  establishment: IContractEstablishment;
  type: ListEntryType;
  lifecycleStep: LifecycleStep;
  attachment?: IEstablishmentAttachment;
}
