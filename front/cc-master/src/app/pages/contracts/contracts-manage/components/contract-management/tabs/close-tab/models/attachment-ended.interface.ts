import { attachmentFields, IAttachment } from '@cc/domain/billing/attachments';
import { establishmentFields, IEstablishment } from '@cc/domain/billing/establishments';

export const attachmentEndedFields = [
  attachmentFields,
  'end',
  `legalEntity[${establishmentFields}]`,
].join(',');

export interface IAttachmentEnded extends IAttachment {
  end: string;
  legalEntity: IEstablishment;
}
