import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3Response } from '@cc/common/queries';
import { IContractForm } from '@cc/domain/billing/contracts';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { contractDetailedFields, IContractDetailed } from '../models/contract-detailed.interface';
import { IContractEditionDto } from '../models/contract-edition-dto.interface';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IClient } from '@cc/domain/billing/clients';

@Injectable()
export class ContractTabService {
  private readonly contractsEndpoint = '/api/v3/contracts';

  constructor(private httpClient: HttpClient) {}

  public getContractDetailed$(id: number): Observable<IContractDetailed> {
    const urlById = `${ this.contractsEndpoint }/${ id }`;
    const params = new HttpParams().set('fields', contractDetailedFields);

    return this.httpClient.get<IHttpApiV3Response<IContractDetailed>>(urlById, { params })
      .pipe(map(response => response.data));
  }

  public deleteContract$(contractId: number): Observable<void> {
    const urlById = `${ this.contractsEndpoint }/${ contractId }`;
    return this.httpClient.delete<void>(urlById);
  }

  public updateContract$(contractId: number, contractForm: IContractForm): Observable<void> {
    const urlById = `${ this.contractsEndpoint }/${ contractId }`;
    const body = this.getContractEditBody(contractForm);
    return this.httpClient.put<void>(urlById, body);
  }

  private getContractEditBody(contractForm: IContractForm): IContractEditionDto {
    return {
      billingMonth: contractForm.billingMonth,
      distributorId: contractForm.distributor.id,
      clientId: contractForm.client.id,
      offerId: contractForm.offer?.id,
      unityNumberTheorical: contractForm.theoreticalDraftCount,
      clientRebate: contractForm.clientRebate.count,
      endClientRebateOn: contractForm.clientRebate.endAt,
      nbMonthTheorical: contractForm.theoreticalMonthRebate,
      theoricalStartOn: contractForm.theoreticalStartOn,
      minimalBillingPercentage: contractForm.minimalBillingPercentage,
      comment: contractForm.comment,
    };
  }
}
