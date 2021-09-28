export enum ContactAdditionalColumn {
  CreatedAt = 'createdAt',
  ExpiresAt = 'expiresAt',
}

export interface IContactAdditionalColumn {
  id: string;
  name: string;
}

export const contactAdditionalColumns: IContactAdditionalColumn[] = [
  {
    id: ContactAdditionalColumn.CreatedAt,
    name: 'cafe_contacts_list_createdAt',
  },
  {
    id: ContactAdditionalColumn.ExpiresAt,
    name: 'cafe_contacts_list_expiredAt',
  },
];
