import { IContactAdditionalColumn } from '../common/components/contact-additional-column-select/contact-additional-column.interface';

export enum ClientContactAdditionalColumn {
  CreatedAt = 'createdAt',
  ExpiresAt = 'expiresAt',
}

export const clientContactAdditionalColumns: IContactAdditionalColumn[] = [
  {
    id: ClientContactAdditionalColumn.CreatedAt,
    name: 'cafe_contacts_list_createdAt',
  },
  {
    id: ClientContactAdditionalColumn.ExpiresAt,
    name: 'cafe_contacts_list_expiredAt',
  },
];
