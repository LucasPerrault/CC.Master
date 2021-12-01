import { IClientWithExternalInformation } from './contract-detailed.interface';

export interface IContractFormInformation {
  distributorRebate: number;
  client: IClientWithExternalInformation;
}
