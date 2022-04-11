import { IAdditionalColumn } from '../../common/models';

export enum EstablishmentAdditionalColumn {
  Name = 'name',
  Environment = 'environment',
  Country = 'country',
}

export const establishmentAdditionalColumns: IAdditionalColumn[] = [
  {
    id: EstablishmentAdditionalColumn.Name,
    name: 'cafe_establishment_list_name',
  },
  {
    id: EstablishmentAdditionalColumn.Environment,
    name: 'cafe_establishment_list_environment',
  },
  {
    id: EstablishmentAdditionalColumn.Country,
    name: 'cafe_establishment_list_country',
  },
];

export const getAdditionalColumnByIds = (ids: EstablishmentAdditionalColumn[]): IAdditionalColumn[] =>
  establishmentAdditionalColumns.filter(c => ids.includes(c.id as EstablishmentAdditionalColumn));

export const getColumnById = (id: EstablishmentAdditionalColumn): IAdditionalColumn => getAdditionalColumnByIds([id])?.[0];
