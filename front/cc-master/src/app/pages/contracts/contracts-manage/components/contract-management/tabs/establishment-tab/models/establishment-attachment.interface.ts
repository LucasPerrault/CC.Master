import { attachmentFields, IAttachment } from '@cc/domain/billing/attachments';

import { AttachmentEndReason } from '../constants/attachment-end-reason.const';
import { establishmentContractFields, IEstablishmentContract } from './establishment-contract.interface';

export const establishmentAttachmentFields = [
  attachmentFields,
  'nbMonthFree',
  'endReason',
  'start',
  'end',
  'legalEntityId',
  'contractID',
  `contract[${ establishmentContractFields }]`,
];

export interface IEstablishmentAttachment extends IAttachment {
  nbMonthFree: number;
  endReason: AttachmentEndReason;
  start: string;
  end: string;
  legalEntityId: number;
  contractID: number;
  contract: IEstablishmentContract;
}
