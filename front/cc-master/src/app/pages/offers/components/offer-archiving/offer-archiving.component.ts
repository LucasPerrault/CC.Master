import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

import { OfferListService } from '../../services/offer-list.service';
import { OffersDataService } from '../../services/offers-data.service';
import { IOfferArchivingModalData } from './offer-archiving.interface';

@Component({
  selector: 'cc-offer-archiving',
  templateUrl: './offer-archiving.component.html',
})
export class OfferArchivingComponent implements ILuModalContent {
  public title: string;
  public submitLabel: string;

  constructor(
    @Inject(LU_MODAL_DATA) public data: IOfferArchivingModalData,
    private translatePipe: TranslatePipe,
    private offerListService: OfferListService,
    private offersDataService: OffersDataService,
  ) {
    const titleTranslationKey = this.getTitleTranslationKey();
    this.title = this.translatePipe.transform(titleTranslationKey, { name: this.data?.offer?.name });

    const submitLabelTranslationKey = this.getSubmitLabelTranslationKey();
    this.submitLabel = this.translatePipe.transform(submitLabelTranslationKey);
  }

  public submitAction(): Observable<void> {
    const request$ = this.data.isArchived
      ? this.offersDataService.archive$(this.data?.offer)
      : this.offersDataService.unarchive$(this.data?.offer);

    return request$.pipe(
      catchError((err: HttpErrorResponse) => throwError(`${ err.status }: ${ err.statusText }`)),
      tap(() => this.offerListService.refresh()),
    );
  }

  private getTitleTranslationKey(): string {
    return this.data.isArchived ? 'offers_archive_title' : 'offers_unarchive_title';
  }

  private getSubmitLabelTranslationKey(): string {
    return this.data.isArchived ? 'offers_archive_button' : 'offers_unarchive_button';
  }
}
