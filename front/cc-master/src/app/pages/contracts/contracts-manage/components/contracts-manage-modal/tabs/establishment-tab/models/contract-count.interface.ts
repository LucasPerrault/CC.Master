import { CountCode, countFields, ICount } from '@cc/domain/billing/counts';

export const establishmentContractCountFields = [
  countFields,
  'code',
].join(',');

export interface IContractCount extends ICount {
  code: CountCode;
}
