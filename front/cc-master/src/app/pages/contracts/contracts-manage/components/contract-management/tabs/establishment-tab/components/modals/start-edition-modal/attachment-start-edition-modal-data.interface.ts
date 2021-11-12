import { IContractEstablishment } from '../../../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../../../models/establishment-attachment.interface';

export interface IAttachmentStartEditionModalData {
  establishments: IContractEstablishment[];
  attachments: IEstablishmentAttachment[];
  lastCountPeriod: Date;
  contractStartDate: Date;
}
