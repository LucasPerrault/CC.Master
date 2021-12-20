import { IAttachment } from '@cc/domain/billing/attachments';

export interface IContractAttachment extends IAttachment {
  id: number;
  contractId: number;
  establishmentId: number;
  establishmentRemoteId: number;
  establishmentName: string;
  startsOn: string;
  endsOn: string;
  isActive: boolean;
}
