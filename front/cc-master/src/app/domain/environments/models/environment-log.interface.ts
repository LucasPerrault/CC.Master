import { IPrincipal } from '@cc/aspects/principal';

import { IEnvironmentLogMessage } from '../../../pages/logs/models/environment-log-message.interface';
import { IEnvironment } from './environment.interface';

export interface IEnvironmentLog {
  id: number;
  name: string;
  userId: number;
  user: IPrincipal;
  environmentId: number;
  environment: IEnvironment;
  activityId: number;
  activity: string;
  isAnonymizedData: boolean;
  createdOn: Date;
  messages: IEnvironmentLogMessage[];
}
