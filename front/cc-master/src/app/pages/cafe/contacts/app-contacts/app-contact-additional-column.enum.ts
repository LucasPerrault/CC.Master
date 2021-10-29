import { IContactAdditionalColumn } from '../common/components/contact-additional-column-select/contact-additional-column.interface';

export enum AppContactAdditionalColumn {
  CreatedAt = 'createdAt',
  ExpiresAt = 'expiresAt',
}

export const appContactAdditionalColumns: IContactAdditionalColumn[] = [
  {
    id: AppContactAdditionalColumn.CreatedAt,
    name: 'cafe_contacts_list_createdAt',
  },
  {
    id: AppContactAdditionalColumn.ExpiresAt,
    name: 'cafe_contacts_list_expiredAt',
  },
];
