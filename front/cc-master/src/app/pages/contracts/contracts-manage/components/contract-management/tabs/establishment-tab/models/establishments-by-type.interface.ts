import { IEstablishmentWithAttachments } from './establishment-with-attachments.interface';

export interface IEstablishmentsWithAttachmentsByType {
  linkedToContract: IEstablishmentWithAttachments[];
  linkedToAnotherContract: IEstablishmentWithAttachments[];
  excluded: IEstablishmentWithAttachments[];
  withError: IEstablishmentWithAttachments[];
}

