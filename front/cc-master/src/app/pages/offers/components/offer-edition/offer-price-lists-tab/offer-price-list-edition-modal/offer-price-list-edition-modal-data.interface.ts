import { IPriceList } from '@cc/domain/billing/offers';

import { IOfferValidationContext } from '../../../../models/offer-validation-context.interface';

export interface IOfferPriceListEditionModalData {
  priceListToEdit: IPriceList;
  validationContext: IOfferValidationContext;
}
