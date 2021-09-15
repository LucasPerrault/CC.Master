import { IContractEstablishment } from './contract-establishment.interface';
import { IEstablishmentAttachment } from './establishment-attachment.interface';

export interface IEstablishmentWithAttachments {
  establishment: IContractEstablishment;
  currentAttachment: IEstablishmentAttachment;
  nextAttachment: IEstablishmentAttachment;
  lastAttachment: IEstablishmentAttachment;
}
