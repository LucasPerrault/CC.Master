import { IPrincipal } from '@cc/aspects/principal';
import { IDateRange } from '@cc/common/date';

export interface ILogsFilter {
  environmentIds: number[];
  domainIds: number[];
  users: IPrincipal[];
  actionIds: number[];
  createdOn: IDateRange;
  isAnonymizedData: string;
}
