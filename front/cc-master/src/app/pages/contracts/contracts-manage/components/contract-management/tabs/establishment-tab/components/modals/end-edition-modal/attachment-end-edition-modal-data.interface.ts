import { IContractEstablishment } from '../../../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../../../models/establishment-attachment.interface';

export enum AttachmentEndEditionModalMode {
  FutureDeactivationEdition = 0,
  Unlinking = 1,
  UnlinkingCancellation = 2,
}

export interface IAttachmentEndEditionModalData {
  establishments: IContractEstablishment[];
  attachments: IEstablishmentAttachment[];
  description?: string;
  lastCountPeriod?: Date;
  mode: AttachmentEndEditionModalMode;
  contractCloseOn: string;
}
