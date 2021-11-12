import { IDetailedOffer } from '../../models/detailed-offer.interface';

export interface IOfferEditionValidationContext {
  realCountNumber: number;
  offer: IDetailedOffer;
}
