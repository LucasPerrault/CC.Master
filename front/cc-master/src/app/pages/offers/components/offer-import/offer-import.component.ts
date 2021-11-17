import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IUploadedOffer } from './uploaded-offer.interface';

@Component({
  selector: 'cc-offer-import',
  templateUrl: './offer-import.component.html',
  styleUrls: ['./offer-import.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OfferImportComponent implements OnInit {

  public uploadedOffers$: BehaviorSubject<IUploadedOffer[]> = new BehaviorSubject([]);

  public get totalCount$(): Observable<number> {
    return this.uploadedOffers$.pipe(map(o => o.length));
  }

  constructor() { }

  ngOnInit(): void {
  }

  public upload(offers: IUploadedOffer[]): void {
    this.uploadedOffers$.next(offers);
  }

}
