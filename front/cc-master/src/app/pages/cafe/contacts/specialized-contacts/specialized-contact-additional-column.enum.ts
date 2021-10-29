import { IContactAdditionalColumn } from '../common/components/contact-additional-column-select/contact-additional-column.interface';

export enum SpecializedContactAdditionalColumn {
  Environment = 'environment',
  LastName = 'lastName',
  FirstName = 'firstName',
  Mail = 'mail',
  Role = 'role',
  IsConfirmed = 'isConfirmed',
  CreatedAt = 'createdAt',
  ExpiresAt = 'expiresAt',
}

export const specializedContactAdditionalColumns: IContactAdditionalColumn[] = [
  {
    id: SpecializedContactAdditionalColumn.Environment,
    name: 'cafe_contacts_list_environment',
  },
  {
    id: SpecializedContactAdditionalColumn.LastName,
    name: 'cafe_contacts_list_lastname',
  },
  {
    id: SpecializedContactAdditionalColumn.FirstName,
    name: 'cafe_contacts_list_firstname',
  },
  {
    id: SpecializedContactAdditionalColumn.Mail,
    name: 'cafe_contacts_list_mail',
  },
  {
    id: SpecializedContactAdditionalColumn.Role,
    name: 'cafe_contacts_list_client',
  },
  {
    id: SpecializedContactAdditionalColumn.IsConfirmed,
    name: 'cafe_contacts_list_isConfirmed',
  },
  {
    id: SpecializedContactAdditionalColumn.CreatedAt,
    name: 'cafe_contacts_list_createdAt',
  },
  {
    id: SpecializedContactAdditionalColumn.ExpiresAt,
    name: 'cafe_contacts_list_expiredAt',
  },
];

export const getAdditionalColumnByIds = (ids: SpecializedContactAdditionalColumn[]): IContactAdditionalColumn[] =>
    specializedContactAdditionalColumns.filter(c => ids.includes(c.id as SpecializedContactAdditionalColumn));
