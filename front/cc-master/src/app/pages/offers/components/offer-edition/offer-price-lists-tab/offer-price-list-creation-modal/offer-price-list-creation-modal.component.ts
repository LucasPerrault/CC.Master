import { ChangeDetectionStrategy, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { addMonths, startOfMonth } from 'date-fns';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { OffersEditionStoreService } from '../../offers-edition-store.service';
import { PriceListValidationError, PriceListsValidators } from '../../../../services/price-lists.validators';
import { IOfferPriceListCreationModalData } from './offer-price-list-creation-modal-data.interface';

enum PriceListFormKey {
  StartsOn = 'startsOn',
  PriceRows = 'rows',
}

@Component({
  selector: 'cc-offer-price-list-creation-modal',
  templateUrl: './offer-price-list-creation-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OfferPriceListCreationModalComponent implements OnInit, OnDestroy, ILuModalContent {
  public title = this.translatePipe.transform('offers_priceList_creation_title');
  public submitLabel = this.translatePipe.transform('offers_priceList_creation_button');
  public submitDisabled = true;

  public formGroup: FormGroup;
  public formKey = PriceListFormKey;
  public validationError = PriceListValidationError;

  public granularity = ELuDateGranularity;

  public get min(): Date {
    return addMonths(startOfMonth(Date.now()), 1);
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    @Inject(LU_MODAL_DATA) public data: IOfferPriceListCreationModalData,
    private translatePipe: TranslatePipe,
    private storeService: OffersEditionStoreService,
  ) {
    this.formGroup = new FormGroup({
      [PriceListFormKey.StartsOn]: new FormControl(),
      [PriceListFormKey.PriceRows]: new FormControl(),
    }, [PriceListsValidators.uniqStartsOn(data.allListStartDates)]);
  }

  public ngOnInit(): void {
    this.formGroup.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.formGroup.invalid);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    const form = this.formGroup.value;
    return this.storeService.createPriceList$(this.data.offerId, form);
  }

}
