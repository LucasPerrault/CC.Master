import { ICountListEntry } from './count-list-entry.interface';

export interface ICountsReplayModalData {
  entries: ICountListEntry[];
  contractId: number;
  max: Date;
  min: Date;
}
