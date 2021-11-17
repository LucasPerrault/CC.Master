import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { addMonths, startOfMonth } from 'date-fns';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { OfferRestrictionsService } from '../../../../services/offer-restrictions.service';
import { OffersEditionStoreService } from '../../offers-edition-store.service';
import { PriceListValidationError, PriceListsValidators } from '../../../../services/price-lists.validators';
import { IOfferPriceListEditionModalData } from './offer-price-list-edition-modal-data.interface';

enum PriceListFormKey {
  StartsOn = 'startsOn',
  PriceRows = 'rows',
}

@Component({
  selector: 'cc-offer-price-list-edition-modal',
  templateUrl: './offer-price-list-edition-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OfferPriceListEditionModalComponent implements OnInit, OnDestroy, ILuModalContent {
  public title: string;
  public submitLabel: string;
  public submitDisabled = true;

  public formGroup: FormGroup;
  public formKey = PriceListFormKey;
  public validationError = PriceListValidationError;

  public granularity = ELuDateGranularity;

  public get canEdit(): boolean {
    return this.restrictionsService.canEdit(this.data.offer);
  }

  public get min(): Date {
    return addMonths(startOfMonth(Date.now()), 1);
  }
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    @Inject(LU_MODAL_DATA) public data: IOfferPriceListEditionModalData,
    private translatePipe: TranslatePipe,
    private storeService: OffersEditionStoreService,
    private datePipe: DatePipe,
    private restrictionsService: OfferRestrictionsService,
  ) {
    const start = datePipe.transform(new Date(this.data.priceListToEdit.startsOn), 'MMMM yyyy');
    this.title = this.translatePipe.transform('offers_priceList_edition_title', { start });
    this.submitLabel = this.translatePipe.transform('offers_priceList_edition_button');

    this.formGroup = new FormGroup({
      [PriceListFormKey.StartsOn]: new FormControl({
        value: new Date(data.priceListToEdit.startsOn),
        disabled: !this.restrictionsService.canEditPriceListStartsOn(data.offer),
      }, [PriceListsValidators.uniqStartsOn(this.data.allListStartDates)]),
      [PriceListFormKey.PriceRows]: new FormControl(data.priceListToEdit.rows),
    },[PriceListsValidators.uniqStartsOn(this.data.allListStartDates)]);
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
    const form = this.formGroup.getRawValue();

    const offerId = this.data.offer.id;
    const priceList = this.data.priceListToEdit;
    return this.storeService.editPriceList$(offerId, priceList, form);
  }
}
