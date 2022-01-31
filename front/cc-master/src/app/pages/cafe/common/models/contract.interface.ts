import { IDistributor } from '@cc/domain/billing/distributors';

import { IEstablishmentContract } from './establishment-contract.interface';

export interface IContract {
  id: number;
  clientId: number;
  externalId: string;
  distributor: IDistributor;

  establishmentAttachments: IEstablishmentContract[];
}
