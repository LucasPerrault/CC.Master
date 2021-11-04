import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BYPASS_INTERCEPTOR } from '@cc/aspects/errors';
import { IHttpApiV3Response } from '@cc/common/queries';
import { contractFields, IContract } from '@cc/domain/billing/contracts';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class ContractManagementDataService {
  private readonly contractsEndPoint = '/api/v3/newcontracts';

  constructor(private httpClient: HttpClient) {
  }

  public getContractById$(id: number): Observable<IContract> {
    const urlById = `${ this.contractsEndPoint }/${ id }`;
    const params = new HttpParams().set('fields', contractFields);
    const context = new HttpContext().set(BYPASS_INTERCEPTOR, true);

    return this.httpClient.get<IHttpApiV3Response<IContract>>(urlById, { params, context })
      .pipe(map(response => response.data));
  }
}
