export enum EnvironmentAdditionalColumn {
  CreatedAt = 'createdAt',
  Countries = 'countries',
  Archived = 'archived',
  Domain = 'domain',
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
    id: EnvironmentAdditionalColumn.Archived,
    name: 'cafe_environments_list_isArchived',
  },
  {
    id: EnvironmentAdditionalColumn.Domain,
    name: 'cafe_environments_list_domain',
  },
];
