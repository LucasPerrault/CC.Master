import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3Response } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { establishmentContractFields, IEstablishmentContract } from '../models/establishment-contract.interface';

@Injectable()
export class EstablishmentContractDataService {
  private readonly contractsEndpoint = `/api/v3/newcontracts`;

  constructor(private httpClient: HttpClient) {}

  public getContract$(contractId: number): Observable<IEstablishmentContract> {
    const urlById = `${ this.contractsEndpoint }/${ contractId }`;
    const params = new HttpParams().set('fields', establishmentContractFields);

    return this.httpClient.get<IHttpApiV3Response<IEstablishmentContract>>(urlById, { params })
      .pipe(map(response => response.data));
  }
}
