import { IContract } from '@cc/domain/billing/contracts';

interface IMiscellaneousTransactionCurrency {
  name: string;
  symbol: string;
  code: number;
}

interface IMiscellaneousTransactionOffer {
  currency: IMiscellaneousTransactionCurrency;
}

export const miscTransactionOfferContractFields = 'startOn,closeOn,offer[currency[name,symbol,code]]';
export interface IMiscellaneousTransactionFormContract extends IContract {
  offer: IMiscellaneousTransactionOffer;
  startOn: string;
  closeOn: string;
}
