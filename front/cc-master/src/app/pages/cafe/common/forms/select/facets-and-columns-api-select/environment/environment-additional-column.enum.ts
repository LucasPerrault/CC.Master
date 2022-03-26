import { IAdditionalColumn } from '../../../../models';
import { EnvironmentCriterionKey } from '../../../../../environments/advanced-filter/environment-criterion-key.enum';

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

export const getAdditionalColumnById = (id: EnvironmentAdditionalColumn): IAdditionalColumn =>
  environmentAdditionalColumns.find(c => c.id === id);

export const getAdditionalColumnByIds = (ids: EnvironmentAdditionalColumn[]): IAdditionalColumn[] =>
    environmentAdditionalColumns.filter(c => ids.includes(c.id as EnvironmentAdditionalColumn));

export const environmentCriterionAndColumnMapping = [
  {
    criterionKey: EnvironmentCriterionKey.AppInstances,
    columnKey: EnvironmentAdditionalColumn.AppInstances,
    column: getAdditionalColumnById(EnvironmentAdditionalColumn.AppInstances),
  },
  {
    criterionKey: EnvironmentCriterionKey.Distributors,
    columnKey: EnvironmentAdditionalColumn.Distributors,
    column: getAdditionalColumnById(EnvironmentAdditionalColumn.Distributors),
  },
  {
    criterionKey: EnvironmentCriterionKey.CreatedAt,
    columnKey: EnvironmentAdditionalColumn.CreatedAt,
    column: getAdditionalColumnById(EnvironmentAdditionalColumn.CreatedAt),
  },
  {
    criterionKey: EnvironmentCriterionKey.Countries,
    columnKey: EnvironmentAdditionalColumn.Countries,
    column: getAdditionalColumnById(EnvironmentAdditionalColumn.Countries),
  },
  {
    criterionKey: EnvironmentCriterionKey.Cluster,
    columnKey: EnvironmentAdditionalColumn.Cluster,
    column: getAdditionalColumnById(EnvironmentAdditionalColumn.Cluster),
  },
  {
    criterionKey: EnvironmentCriterionKey.DistributorType,
    columnKey: EnvironmentAdditionalColumn.DistributorType,
    column: getAdditionalColumnById(EnvironmentAdditionalColumn.DistributorType),
  },
];

