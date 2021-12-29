import { IContract } from '@cc/domain/billing/contracts';

export interface IOfferContract extends IContract {
  theoreticalStartOn: string;
  commercialOfferId: number;
}
