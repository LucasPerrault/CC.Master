import { IDetailedOffer } from './detailed-offer.interface';

export interface IOfferValidationContext {
  realCountNumber: number;
  offer: IDetailedOffer;
}
