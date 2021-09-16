import { ContractBillingPeriodicity } from '../enums/contract-billing-periodicity.enum';
import { ContractEndReason } from '../enums/contract-end-reason.enum';
import { IContractAttachment } from './contract-attachment.interface';
import { IContractClient } from './contract-client.interface';
import { IContractCommercialOffer } from './contract-commercial-offer.interface';
import { IContractDistributor } from './contract-distributor.interface';

export interface IContract {
  id: number;
  externalId: string;
  clientExternalId: string;
  environmentId: number;
  environmentSubdomain: string;
  createdAt: string;
  theoreticalStartOn: string;
  theoreticalEndOn: string;
  endReason: ContractEndReason;
  archivedAt?: boolean;
  distributorId: number;
  distributor: IContractDistributor;
  clientId: number;
  client: IContractClient;
  commercialOfferId: number;
  commercialOffer: IContractCommercialOffer;
  attachments: IContractAttachment[];
  countEstimation: number;
  theoreticalFreeMonths: number;
  rebatePercentage: number;
  rebateEndsOn: string;
  minimalBillingPercentage: number;
  billingPeriodicity: ContractBillingPeriodicity;
}
