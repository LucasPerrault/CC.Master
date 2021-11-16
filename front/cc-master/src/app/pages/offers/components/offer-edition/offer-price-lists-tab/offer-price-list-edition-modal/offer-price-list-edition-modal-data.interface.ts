import { IPriceList } from '@cc/domain/billing/offers';

import { IOfferEditionValidationContext } from '../../offer-edition-validation-context.interface';

export interface IOfferPriceListEditionModalData {
  priceListToEdit: IPriceList;
  validationContext: IOfferEditionValidationContext,
}
