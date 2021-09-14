import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService, IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { subMonths } from 'date-fns';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IContractEnvironmentDetailed } from '../models/contract-environment-detailed.interface';
import { IPreviousContractEnvironment, previousContractEnvironmentFields } from '../models/previous-contract-environment.interface';

@Injectable()
export class EnvironmentCreationCauseService {
  private readonly contractEndpoint = '/api/v3/newcontracts';
  constructor(private httpClient: HttpClient, private apiV3DateService: ApiV3DateService) {}

  public getPreviousContracts$(environmentId: number, contract: IContractEnvironmentDetailed): Observable<IPreviousContractEnvironment[]> {
    const theoreticalStartDate = new Date(contract.theoricalStartOn);
    const solutionIds = contract.product.solutions.map(s => s.id);

    const params = new HttpParams()
      .set('fields', previousContractEnvironmentFields)
      .set('environmentId', String(environmentId))
      .set('distributorId', contract.distributorId)
      .set('endOn', this.apiV3DateService.since(subMonths(theoreticalStartDate, 2)))
      .set('product.solutions.id', solutionIds.join(','));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IPreviousContractEnvironment>>(this.contractEndpoint, { params })
      .pipe(map(response => response.data.items));
  }
}
