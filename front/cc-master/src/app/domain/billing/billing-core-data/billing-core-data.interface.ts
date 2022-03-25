import { IBillingEntity } from '@cc/domain/billing/clients';

export interface IBillingCoreData
{
  billingEntities: IBillingEntity[];
}

export const getNameByCode = (entities: IBillingEntity[], code: string) => entities.find(e => e.code === code)?.name || code;
export const getNameById = (entities: IBillingEntity[], id: number) => entities.find(e => e.id === id)?.name || `${id}`;
