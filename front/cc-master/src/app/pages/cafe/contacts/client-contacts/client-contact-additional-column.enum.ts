import { IContactAdditionalColumn } from '../common/components/contact-additional-column-select/contact-additional-column.interface';

export enum ClientContactAdditionalColumn {
  Environment = 'environment',
  LastName = 'lastName',
  FirstName = 'firstName',
  Mail = 'mail',
  Client = 'client',
  CreatedAt = 'createdAt',
  ExpiresAt = 'expiresAt',
}

export const clientContactAdditionalColumns: IContactAdditionalColumn[] = [
  {
    id: ClientContactAdditionalColumn.Environment,
    name: 'cafe_contacts_list_environment',
  },
  {
    id: ClientContactAdditionalColumn.LastName,
    name: 'cafe_contacts_list_lastname',
  },
  {
    id: ClientContactAdditionalColumn.FirstName,
    name: 'cafe_contacts_list_firstname',
  },
  {
    id: ClientContactAdditionalColumn.Mail,
    name: 'cafe_contacts_list_mail',
  },
  {
    id: ClientContactAdditionalColumn.Client,
    name: 'cafe_contacts_list_client',
  },
  {
    id: ClientContactAdditionalColumn.CreatedAt,
    name: 'cafe_contacts_list_createdAt',
  },
  {
    id: ClientContactAdditionalColumn.ExpiresAt,
    name: 'cafe_contacts_list_expiredAt',
  },
];

export const getAdditionalColumnByIds = (ids: ClientContactAdditionalColumn[]): IContactAdditionalColumn[] =>
    clientContactAdditionalColumns.filter(c => ids.includes(c.id as ClientContactAdditionalColumn));
