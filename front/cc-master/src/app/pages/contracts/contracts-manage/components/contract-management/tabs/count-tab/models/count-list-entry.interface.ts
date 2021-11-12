import { IDetailedCount } from './detailed-count.interface';

export interface ICountListEntry {
  month: Date;
  count?: IDetailedCount;
  buttonStateClass?: string;
}
