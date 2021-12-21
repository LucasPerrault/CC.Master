import { BillingEntity } from '@cc/domain/billing/clients';

export interface IContractDraftFormInformation {
  externalDistributorUrl: string;
  externalOfferName: string;
  externalDeploymentAt: Date;
  clientBillingEntity: BillingEntity;
}
