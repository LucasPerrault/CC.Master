import { IUser } from '@cc/domain/users';

import { IEnvironment } from './environment.interface';
import { IEnvironmentLogMessage } from './environment-log-message.interface';

export interface IEnvironmentLog {
  id: number;
  name: string;
  userId: number;
  user: IUser;
  environmentId: number;
  environment: IEnvironment;
  activityId: number;
  activity: string;
  isAnonymizedData: boolean;
  createdOn: Date;
  messages: IEnvironmentLogMessage[];
}
