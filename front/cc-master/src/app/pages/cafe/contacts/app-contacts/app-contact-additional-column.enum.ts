import { IContactAdditionalColumn } from '../common/components/contact-additional-column-select/contact-additional-column.interface';

export enum AppContactAdditionalColumn {
  Environment = 'environment',
  LastName = 'lastName',
  FirstName = 'firstName',
  Mail = 'mail',
  AppInstance = 'appInstance',
  IsConfirmed = 'isConfirmed',
  CreatedAt = 'createdAt',
  ExpiresAt = 'expiresAt',
}

export const appContactAdditionalColumns: IContactAdditionalColumn[] = [
  {
    id: AppContactAdditionalColumn.Environment,
    name: 'cafe_contacts_list_environment',
  },
  {
    id: AppContactAdditionalColumn.LastName,
    name: 'cafe_contacts_list_lastname',
  },
  {
    id: AppContactAdditionalColumn.FirstName,
    name: 'cafe_contacts_list_firstname',
  },
  {
    id: AppContactAdditionalColumn.Mail,
    name: 'cafe_contacts_list_mail',
  },
  {
    id: AppContactAdditionalColumn.AppInstance,
    name: 'cafe_contacts_list_app',
  },
  {
    id: AppContactAdditionalColumn.IsConfirmed,
    name: 'cafe_contacts_list_isConfirmed',
  },
  {
    id: AppContactAdditionalColumn.CreatedAt,
    name: 'cafe_contacts_list_createdAt',
  },
  {
    id: AppContactAdditionalColumn.ExpiresAt,
    name: 'cafe_contacts_list_expiredAt',
  },
];

export const getAdditionalColumnByIds = (ids: AppContactAdditionalColumn[]): IContactAdditionalColumn[] =>
    appContactAdditionalColumns.filter(c => ids.includes(c.id as AppContactAdditionalColumn));
