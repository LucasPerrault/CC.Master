import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CountsService, ICount } from '@cc/domain/billing/counts';
import { Observable } from 'rxjs';

import { IOfferContract } from '../models/offer-contract.interface';

@Injectable()
export class OfferTabDataService {
  constructor(private httpClient: HttpClient, private countsService: CountsService) {
  }

  public getRealCounts$(contractId: number): Observable<ICount[]> {
    return this.countsService.getRealCounts$(contractId);
  }

  public getContract$(contractId: number): Observable<IOfferContract> {
    const url = `/api/contracts/${ contractId }`;
    return this.httpClient.get<IOfferContract>(url);
  }

  public editContract$(contractId: number, offerId: number): Observable<void> {
    const url = `/api/v3/newcontracts/${ contractId }/offer`;
    const body = { offerId };
    return this.httpClient.put<void>(url, body);
  }
}
