import { IDateRange } from '@cc/common/date';
import { IEnvironment, IEnvironmentAction, IEnvironmentDomain } from '@cc/domain/environments';
import { IUser } from '@cc/domain/users';

export interface ILogsFilter {
  environments: IEnvironment[];
  domains: IEnvironmentDomain[];
  users: IUser[];
  actions: IEnvironmentAction[];
  createdOn: IDateRange;
  isAnonymized?: boolean;
}
