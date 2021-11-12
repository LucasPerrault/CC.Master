export interface IEnvironmentAccess {
  id: number;
  environmentId: number;
  distributorId: number;
  distributor: IDistributor;
}

export interface IDistributor {
  id: number;
  name: string;
}
