import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable } from 'rxjs';

import { PriceListsDataService } from '../../../../services/price-lists-data.service';
import { IOfferPriceListDeletionModalData } from './offer-price-list-deletion-modal-data.interface';

@Component({
  selector: 'cc-offer-price-list-deletion-modal',
  templateUrl: './offer-price-list-deletion-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OfferPriceListDeletionModalComponent implements ILuModalContent {
  public title: string;
  public submitLabel: string;

  public get start(): string {
    return this.datePipe.transform(new Date(this.data.priceListStartsOn), 'MMMM yyyy');
  }

  constructor(
    @Inject(LU_MODAL_DATA) private data: IOfferPriceListDeletionModalData,
    private translatePipe: TranslatePipe,
    private dataService: PriceListsDataService,
    private datePipe: DatePipe,
  ) {
    this.title = this.translatePipe.transform('offers_priceList_deletion_title', { start: this.start });
    this.submitLabel = this.translatePipe.transform('offers_priceList_deletion_button');
  }

  public submitAction(): Observable<void> {
    return this.dataService.delete$(this.data.offerId, this.data.priceListId);
  }

}
