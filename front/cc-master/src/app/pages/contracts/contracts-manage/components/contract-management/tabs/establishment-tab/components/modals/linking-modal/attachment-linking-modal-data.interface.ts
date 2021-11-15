import { IContractEstablishment } from '../../../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../../../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../../../models/establishment-contract.interface';

export interface IAttachmentLinkingModalData {
  establishments: IContractEstablishment[];
  attachments: IEstablishmentAttachment[];
  contract: IEstablishmentContract;
}
