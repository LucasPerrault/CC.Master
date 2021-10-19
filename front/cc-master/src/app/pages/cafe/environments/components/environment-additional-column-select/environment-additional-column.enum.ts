export enum EnvironmentAdditionalColumn {
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
