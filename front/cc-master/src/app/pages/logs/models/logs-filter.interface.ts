import { IPrincipal } from '@cc/aspects/principal';
import { IDateRange } from '@cc/common/date';
import { IEnvironment, IEnvironmentAction } from '@cc/domain/environments';

export interface ILogsFilter {
  environments: IEnvironment[];
  domainIds: number[];
  users: IPrincipal[];
  actions: IEnvironmentAction[];
  createdOn: IDateRange;
  isAnonymizedData: string;
}
