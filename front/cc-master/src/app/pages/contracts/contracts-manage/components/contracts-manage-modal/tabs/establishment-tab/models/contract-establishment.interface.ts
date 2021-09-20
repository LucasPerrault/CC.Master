import { establishmentFields, IEstablishment } from '@cc/domain/billing/establishments';

import { establishmentAttachmentFields, IEstablishmentAttachment } from './establishment-attachment.interface';
import { excludedEntitiesFields, IEstablishmentExcludedEntity } from './establishment-excluded-entity.interface';

export const contractEstablishmentFields = [
  establishmentFields,
  `isActive`,
  `contractEntities[${ establishmentAttachmentFields }]`,
  `excludedEntities[${ excludedEntitiesFields }]`,
].join(',');

export interface IContractEstablishment extends IEstablishment {
  isActive: boolean;
  contractEntities: IEstablishmentAttachment[];
  excludedEntities: IEstablishmentExcludedEntity[];
}
