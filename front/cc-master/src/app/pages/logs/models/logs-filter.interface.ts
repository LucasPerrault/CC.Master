import { IPrincipal } from '@cc/aspects/principal';
import { IDateRange } from '@cc/common/date';
import { IEnvironment } from '@cc/domain/environments';

export interface ILogsFilter {
  environments: IEnvironment[];
  domainIds: number[];
  users: IPrincipal[];
  actionIds: number[];
  createdOn: IDateRange;
  isAnonymizedData: string;
}
