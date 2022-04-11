import { IAdditionalColumn } from '../../common/models';

export enum EnvironmentAdditionalColumn {
  Environment = 'environment',
  AppInstances = 'appInstances',
  Distributors = 'distributors',
  CreatedAt = 'createdAt',
  Countries = 'countries',
  Cluster = 'cluster',
  DistributorType = 'distributorType',
}

export const environmentAdditionalColumns: IAdditionalColumn[] = [
  {
    id: EnvironmentAdditionalColumn.Environment,
    name: 'cafe_environments_list_environment',
  },
  {
    id: EnvironmentAdditionalColumn.AppInstances,
    name: 'cafe_environments_list_applications',
  },
  {
    id: EnvironmentAdditionalColumn.Distributors,
    name: 'cafe_environments_list_distributors',
  },
  {
    id: EnvironmentAdditionalColumn.CreatedAt,
    name: 'cafe_environments_list_createdAt',
  },
  {
    id: EnvironmentAdditionalColumn.Countries,
    name: 'cafe_environments_list_countries',
  },
  {
    id: EnvironmentAdditionalColumn.Cluster,
    name: 'cafe_environments_list_cluster',
  },
  {
    id: EnvironmentAdditionalColumn.DistributorType,
    name: 'cafe_environments_list_distributorType',
  },
];

export const getAdditionalColumnByIds = (ids: EnvironmentAdditionalColumn[]): IAdditionalColumn[] =>
    environmentAdditionalColumns.filter(c => ids.includes(c.id as EnvironmentAdditionalColumn));

export const getColumnById = (id: EnvironmentAdditionalColumn): IAdditionalColumn => getAdditionalColumnByIds([id])?.[0];
