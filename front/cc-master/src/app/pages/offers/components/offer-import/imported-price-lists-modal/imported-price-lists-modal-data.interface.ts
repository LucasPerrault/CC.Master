import { IPriceListForm } from '../../../models/price-list-form.interface';

export interface IImportedPriceListsModalData {
  priceLists: IPriceListForm[];
  readonly: boolean;
}
