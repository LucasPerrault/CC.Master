import { contractFields, IContract } from '@cc/domain/billing/contracts';
import { countFields, ICount } from '@cc/domain/billing/counts';

export const countWithContractFields = `${ countFields },contract[${ contractFields }]`;
export interface ICountWithContract extends ICount {
  contract: IContract;
}
