import { IContractCreationDto } from '@cc/domain/billing/contracts';

export interface IContractConversionDto {
  contractDrafts: IContractCreationFromOpportunityDto[];
}

interface IContractCreationFromOpportunityDto extends IContractCreationDto {
  opportunityLineItemReccuringId: string;
}

export interface IContractDeletionDto {
  contractDrafts: IContractDeletionFromOpportunityDto[];
}
interface IContractDeletionFromOpportunityDto {
  opportunityLineItemReccuringId: string;
}
