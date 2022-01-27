import { attachmentFields, IAttachment } from '@cc/domain/billing/attachments';
import { establishmentFields, IEstablishment } from '@cc/domain/billing/establishments';

export interface IClosureFormValidationContext {
  theoreticalStartOn: Date;
  lastCountPeriod?: Date;
  mostRecentAttachment?: IContextAttachment;
}

export const contextAttachmentFields = [attachmentFields,'start','end',`legalEntity[${establishmentFields}]`].join(',');
export interface IContextAttachment extends IAttachment {
  start: string;
  end: string;
  legalEntity: IEstablishment;
}
