import { IDetailedOffer } from '../../models/detailed-offer.interface';

export interface IOfferArchivingModalData {
  offer: IDetailedOffer;
  isArchived: boolean;
}
