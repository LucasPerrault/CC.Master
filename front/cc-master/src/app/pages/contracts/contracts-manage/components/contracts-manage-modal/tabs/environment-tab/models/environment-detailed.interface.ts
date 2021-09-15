import { environmentFields, IEnvironment } from '@cc/domain/environments';

export const environmentDetailedFields = `${ environmentFields },name`;

export interface IEnvironmentDetailed extends IEnvironment {
  name: string;
}
