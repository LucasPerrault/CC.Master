import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3Response } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ISubdomainAvailability, subdomainAvailabilityFields } from './subdomain-availability.interface';
import { SubdomainAvailabilityStatus } from './subdomain-availability-status.enum';

@Injectable()
export class SubdomainAvailabilityDataService {

  constructor(private httpClient: HttpClient) {}

  public getStatus$(subdomain: string): Observable<SubdomainAvailabilityStatus> {
    const url = '/api/v3/subdomains/availability';
    const params = new HttpParams()
      .set('fields', subdomainAvailabilityFields)
      .set('subdomain', subdomain);

    return this.httpClient.get<IHttpApiV3Response<ISubdomainAvailability>>(url, { params })
      .pipe(map(res => res.data.status));
  }
}
