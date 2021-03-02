import { IPrincipal } from '@cc/aspects/principal';
import { IDateRange } from '@cc/common/date';
import { IEnvironment, IEnvironmentAction, IEnvironmentDomain } from '@cc/domain/environments';

export interface ILogsFilter {
  environments: IEnvironment[];
  domains: IEnvironmentDomain[];
  users: IPrincipal[];
  actions: IEnvironmentAction[];
  createdOn: IDateRange;
  isAnonymized?: boolean;
}
