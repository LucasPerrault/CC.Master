export interface IEnvironmentAccess {
  id: number;
  environmentId: number;
  distributorId: number;
  distributor: IDistributor;
}

export interface IDistributor {
  isLucca: boolean;
  id: number;
  name: string;
}
