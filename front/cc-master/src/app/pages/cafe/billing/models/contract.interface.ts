import { IEstablishmentContract } from './establishment-contract.interface';

export interface IContract {
  id: number;
  clientId: number;
  externalId: string;

  establishmentAttachments: IEstablishmentContract[];
}
