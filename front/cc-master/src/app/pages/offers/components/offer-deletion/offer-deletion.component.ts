import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ToastsService, ToastType } from '@cc/common/toasts';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

import { IDetailedOffer } from '../../models/detailed-offer.interface';
import { OfferListService } from '../../services/offer-list.service';
import { OffersDataService } from '../../services/offers-data.service';

@Component({
  selector: 'cc-offer-deletion',
  templateUrl: './offer-deletion.component.html',
})
export class OfferDeletionComponent implements ILuModalContent {
  public title = this.translatePipe.transform('offers_deletion_title', { name: this.offer.name });
  public submitLabel = this.translatePipe.transform('offers_deletion_button');

  constructor(
    @Inject(LU_MODAL_DATA) public offer: IDetailedOffer,
    private translatePipe: TranslatePipe,
    private toastsService: ToastsService,
    private offerListService: OfferListService,
    private offersDataService: OffersDataService,
  ) { }

  public submitAction(): Observable<void> {
    return this.offersDataService.delete$(this.offer.id).pipe(
      catchError((err: HttpErrorResponse) => throwError(`${ err.status }: ${ err.statusText }`)),
      tap(() => {
        this.offerListService.refresh();
        this.notifySuccessDeletion();
      }),
    );
  }

  private notifySuccessDeletion(): void {
    this.toastsService.addToast({
      type: ToastType.Success,
      message: this.translatePipe.transform('offers_deletion_successMessage', { offerId: this.offer.id }),
    });
  }
}
