import { IContactAdditionalColumn } from '../common/components/contact-additional-column-select/contact-additional-column.interface';

export enum GenericContactAdditionalColumn {
  CreatedAt = 'createdAt',
  ExpiresAt = 'expiresAt',
}

export const genericContactAdditionalColumns: IContactAdditionalColumn[] = [
  {
    id: GenericContactAdditionalColumn.CreatedAt,
    name: 'cafe_contacts_list_createdAt',
  },
  {
    id: GenericContactAdditionalColumn.ExpiresAt,
    name: 'cafe_contacts_list_expiredAt',
  },
];
