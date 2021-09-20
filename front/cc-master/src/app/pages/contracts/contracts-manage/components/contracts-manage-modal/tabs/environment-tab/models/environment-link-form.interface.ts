import { IEnvironment } from '@cc/domain/environments';

import { CreationCause } from '../constants/creation-cause.enum';

export interface IEnvironmentLinkForm {
  environment: IEnvironment;
  creationCause: CreationCause;
}
