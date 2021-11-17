import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IUploadedOffer } from '../uploaded-offer.interface';

@Injectable()
export class OfferUploadService {

  constructor(private httpClient: HttpClient) {
  }

  public upload$(file: any): Observable<IUploadedOffer[]> {
    const url = '/api/commercial-offers/upload';

    const formData = new FormData();
    formData.append('file', file);

    return this.httpClient.post<IUploadedOffer[]>(url, formData);
  }
}
