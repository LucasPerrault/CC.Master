import { IPrincipal } from '@cc/aspects/principal';

import { IEnvironment } from '../../../pages/logs/models/environment.interface';
import { IEnvironmentLogMessage } from '../../../pages/logs/models/environment-log-message.interface';

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
