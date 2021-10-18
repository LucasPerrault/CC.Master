import { IContract } from './contract.interface';

export interface IClient {
  id: number;
  externalId: string;
  name: string;

  contracts: IContract[];
}
