import { BillingEntity } from './billing-entity.interface';

export const clientFields = 'id,name,billingEntity';
export interface IClient {
  id: number;
  name: string;
  billingEntity: BillingEntity;
}
