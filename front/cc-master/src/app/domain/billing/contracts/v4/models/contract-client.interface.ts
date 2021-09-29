import { IClient } from '@cc/domain/billing/clients';

export interface IContractClient extends IClient {
  externalId: string;
  salesforceId: string;
}
