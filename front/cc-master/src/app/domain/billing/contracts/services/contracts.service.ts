import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IContract } from '../models/contract.interface';
import { IContractCreationDto } from '../models/contract-creation-dto.interface';
import { IContractForm } from '../models/contract-form.interface';

@Injectable()
export class ContractsService {
  private readonly newContractsEndPoint = '/api/v3/newcontracts';

  constructor(private httpClient: HttpClient) {
  }

  public createContract$(contractForm: IContractForm): Observable<IContract> {
    const body = this.getContractPostBody(contractForm);
    return this.httpClient.post<IContract>(this.newContractsEndPoint, body);
  }

  private getContractPostBody(contractForm: IContractForm): IContractCreationDto {
    return {
      billingMonth: contractForm.billingMonth,
      distributorId: contractForm.distributor?.id,
      clientId: contractForm.client?.id,
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
