import { BillingEntity } from '@cc/domain/billing/clients';

export interface IClientInfoModalData {
  name: string;
  salesforceId: string;
  commercialManagementId: string;
  billingEntity: BillingEntity;
}
