import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BYPASS_INTERCEPTOR } from '@cc/aspects/errors';
import { IHttpApiV3Response } from '@cc/common/queries';
import { contractFields, IContract } from '@cc/domain/billing/contracts';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

class ContractRoute {
  public static base = '/api/v3/newcontracts';
  public static legacy = '/api/v3/contracts';
  public static id = (contractId: number) => `${ ContractRoute.base }/${ contractId }`;
  public static legacyId = (contractId: number) => `${ ContractRoute.legacy }/${ contractId }`;
}

@Injectable()
export class ContractManagementDataService {
  constructor(private httpClient: HttpClient) {
  }

  public deleteContract$(contractId: number): Observable<void> {
    const urlById = ContractRoute.legacyId(contractId);
    return this.httpClient.delete<void>(urlById);
  }

  public getContractById$(id: number): Observable<IContract> {
    const urlById = ContractRoute.id(id);
    const params = new HttpParams().set('fields', contractFields);
    const context = new HttpContext().set(BYPASS_INTERCEPTOR, true);

    return this.httpClient.get<IHttpApiV3Response<IContract>>(urlById, { params, context })
      .pipe(map(response => response.data));
  }
}
