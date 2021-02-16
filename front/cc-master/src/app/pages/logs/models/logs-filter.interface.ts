import { IDateRange } from '@cc/common/date';

export interface ILogsFilter {
  environmentIds: number[];
  domainIds: number[];
  userIds: number[];
  actionIds: number[];
  createdOn: IDateRange;
  isAnonymizedData: string;
}
