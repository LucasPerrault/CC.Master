import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse, IHttpApiV3Response } from '@cc/common/queries';
import { CountCode } from '@cc/domain/billing/counts';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { establishmentContractCountFields, IContractCount } from '../models/contract-count.interface';
import { establishmentContractFields, IEstablishmentContract } from '../models/establishment-contract.interface';

@Injectable()
export class EstablishmentContractDataService {
  private readonly contractsEndpoint = `/api/v3/newcontracts`;
  private readonly countsEndpoint = `/api/v3/counts`;

  constructor(private httpClient: HttpClient) {}

  public getContract$(contractId: number): Observable<IEstablishmentContract> {
    const urlById = `${ this.contractsEndpoint }/${ contractId }`;
    const params = new HttpParams().set('fields', establishmentContractFields);

    return this.httpClient.get<IHttpApiV3Response<IEstablishmentContract>>(urlById, { params })
      .pipe(map(response => response.data));
  }

  public getRealCounts$(contractId: number): Observable<IContractCount[]> {
    const params = new HttpParams()
      .set('fields', establishmentContractCountFields)
      .set('code', CountCode.Count)
      .set('contractId', `${ contractId }`);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IContractCount>>(this.countsEndpoint, { params })
      .pipe(map(res => res.data.items));
  }
}
