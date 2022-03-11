import { countFields, ICount } from '@cc/domain/billing/counts';

export const countWithContractFields = `${ countFields },contractID`;
export interface ICountWithContract extends ICount {
  contractID: number;
}
