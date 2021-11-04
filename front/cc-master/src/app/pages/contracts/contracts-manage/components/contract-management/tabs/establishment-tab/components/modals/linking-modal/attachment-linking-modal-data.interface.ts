import { IContractEstablishment } from '../../../models/contract-establishment.interface';
import { IEstablishmentContract } from '../../../models/establishment-contract.interface';

export interface IAttachmentLinkingModalData {
  establishments: IContractEstablishment[];
  contract: IEstablishmentContract;
}
