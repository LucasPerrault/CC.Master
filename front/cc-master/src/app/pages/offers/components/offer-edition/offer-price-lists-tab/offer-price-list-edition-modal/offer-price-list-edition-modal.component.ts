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
import { PriceListsValidators, PriceListValidationError } from '../../../../services/price-lists.validators';
import { PriceListsDataService } from '../../../../services/price-lists-data.service';
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

  public get min(): Date {
    return addMonths(startOfMonth(Date.now()), 1);
  }
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    @Inject(LU_MODAL_DATA) public data: IOfferPriceListEditionModalData,
    private translatePipe: TranslatePipe,
    private dataService: PriceListsDataService,
    private datePipe: DatePipe,
    private restrictionsService: OfferRestrictionsService,
  ) {
    const start = datePipe.transform(new Date(this.data.priceListToEdit.startsOn), 'MMMM yyyy');
    this.title = this.translatePipe.transform('offers_priceList_edition_title', { start });
    this.submitLabel = this.translatePipe.transform('offers_priceList_edition_button');

    const priceListsWithoutEdition = data.validationContext.offer.priceLists.filter(p => p.id !== data.priceListToEdit.id);
    this.formGroup = new FormGroup({
      [PriceListFormKey.StartsOn]: new FormControl({
        value: new Date(data.priceListToEdit.startsOn),
        disabled: !this.restrictionsService.canEditPriceListStartsOn(data.validationContext),
      }, [PriceListsValidators.uniqStartsOn(priceListsWithoutEdition)]),
      [PriceListFormKey.PriceRows]: new FormControl({
        value: data.priceListToEdit.rows,
        disabled: !this.restrictionsService.canEdit(data.validationContext),
      }),
    });
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

    const offerId = this.data.validationContext.offer.id;
    const priceListId = this.data.priceListToEdit.id;
    return this.dataService.edit$(offerId, priceListId, form);
  }
}
