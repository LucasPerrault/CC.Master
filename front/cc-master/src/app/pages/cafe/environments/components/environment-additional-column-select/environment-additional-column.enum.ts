export enum EnvironmentAdditionalColumn {
  Environment = 'environment',
  AppInstances = 'appInstances',
  Distributors = 'distributors',
  CreatedAt = 'createdAt',
  Countries = 'countries',
  Cluster = 'cluster',
}

export interface IEnvironmentAdditionalColumn {
  id: EnvironmentAdditionalColumn;
  name: string;
}

export const environmentAdditionalColumns: IEnvironmentAdditionalColumn[] = [
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
];

export const getAdditionalColumnByIds = (ids: EnvironmentAdditionalColumn[]): IEnvironmentAdditionalColumn[] => {
  return environmentAdditionalColumns.filter(c => ids.includes(c.id));
};

