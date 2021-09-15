import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService, IHttpApiV3Response } from '@cc/common/queries';
import { endOfMonth } from 'date-fns';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ICloseContractReason } from '../models/close-contract-reason.interface';
import { contractClosureDetailedFields, IContractClosureDetailed } from '../models/contract-closure-detailed.interface';
import { IContractClosureDto } from '../models/contract-closure-dto.interface';

@Injectable()
export class CloseContractService {
  private readonly legacyContractsEndpoint = '/api/v3/contracts';
  private readonly contractsEndpoint = '/api/v3/newcontracts';

  constructor(private httpClient: HttpClient, private apiV3DateService: ApiV3DateService) {}

  public getContractClosureDetailed$(contractId: number): Observable<IContractClosureDetailed> {
    const urlById = `${ this.contractsEndpoint }/${ contractId }`;
    const params = new HttpParams().set('fields', contractClosureDetailedFields);

    return this.httpClient.get<IHttpApiV3Response<IContractClosureDetailed>>(urlById, { params })
      .pipe(map(response => response.data));
  }

  public closeContract$(contractId: number, closeOn: string, closeReason: ICloseContractReason): Observable<void> {
    const urlById = `${ this.legacyContractsEndpoint }/${ contractId }/Close`;
    return this.httpClient.put<void>(urlById, this.getCloseContractBody(closeOn, closeReason));
  }

  public cancelContractClosure$(contractId: number): Observable<void> {
    const urlById = `${ this.legacyContractsEndpoint }/${ contractId }`;

    return this.httpClient.put<void>(urlById, {
      closeOn: null,
      closeReason: null,
    });
  }

  private getCloseContractBody(closeOn: string, closeReason: ICloseContractReason): IContractClosureDto {
    return {
      closeOn: this.apiV3DateService.toApiV3DateFormat(endOfMonth(new Date(closeOn))),
      closeReason: closeReason.id,
    };
  }
}
