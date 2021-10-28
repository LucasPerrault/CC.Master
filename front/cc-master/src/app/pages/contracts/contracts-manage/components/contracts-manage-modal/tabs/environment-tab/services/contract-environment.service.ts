import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse, IHttpApiV3CountResponse, IHttpApiV3Response } from '@cc/common/queries';
import { from, Observable, pipe, UnaryFunction } from 'rxjs';
import { concatMap, distinct, filter, map, reduce, switchMap } from 'rxjs/operators';

import { CreationCause } from '../constants/creation-cause.enum';
import { contractEnvironmentFields, IContractEnvironmentDetailed } from '../models/contract-environment-detailed.interface';
import { environmentDetailedFields, IEnvironmentDetailed } from '../models/environment-detailed.interface';

@Injectable()
export class ContractEnvironmentService {
  private readonly contractEndpoint = '/api/v3/newcontracts';
  private readonly attachmentsEndpoint = '/api/v3/contractentities';
  private readonly environmentsEndpoint = '/api/v3/environments';

  constructor(private httpClient: HttpClient) {}

  public getSuggestedEnvironments$(clientId: number): Observable<IEnvironmentDetailed[]> {
    return this.getContractEnvironmentsByClient$(clientId).pipe(this.toEnvironments);
  }

  public unlinkEnvironment$(contractId: number): Observable<void> {
    const urlById = `${ this.contractEndpoint }/${ contractId }`;
    return this.httpClient.put<void>(urlById, { environmentId: null });
  }

  public linkEnvironment$(contractId: number, environmentId: number, creationCause: CreationCause): Observable<void> {
    const urlById = `${ this.contractEndpoint }/${ contractId }`;
    return this.httpClient.put<void>(urlById, { environmentId, creationCause });
  }

  public getAttachmentsNumber$(contractId: number): Observable<number> {
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('isActive', `${ true }`)
      .set('contractId', String(contractId));

    return this.httpClient.get<IHttpApiV3CountResponse>(this.attachmentsEndpoint, { params })
      .pipe(map(response => response.data.count));
  }

  public getContractEnvironment$(contractId: number): Observable<IContractEnvironmentDetailed> {
    const urlById = `${ this.contractEndpoint }/${ contractId }`;
    const params = new HttpParams().set('fields', contractEnvironmentFields);

    return this.httpClient.get<IHttpApiV3Response<IContractEnvironmentDetailed>>(urlById, { params })
      .pipe(map(response => response.data));
  }

  private getContractEnvironmentsByClient$(clientId: number): Observable<IContractEnvironmentDetailed[]> {
    const params = new HttpParams()
      .set('fields', contractEnvironmentFields)
      .set('clientId', String(clientId));
    return this.httpClient.get<IHttpApiV3CollectionResponse<IContractEnvironmentDetailed>>(this.contractEndpoint, { params })
      .pipe(map(response => response.data.items));
  }

  private get toEnvironments(): UnaryFunction<Observable<IContractEnvironmentDetailed[]>, Observable<IEnvironmentDetailed[]>> {
    return pipe(
      concatMap(from),
      filter((contract: IContractEnvironmentDetailed) => !!contract.environment),
      map((contract: IContractEnvironmentDetailed) => contract.environment),
      distinct((environment: IEnvironmentDetailed) => environment.id),
      reduce((environmentIds: number[], env: IEnvironmentDetailed) => [...environmentIds, env.id], []),
      filter(environmentIds => !!environmentIds.length),
      switchMap(environmentIds => this.getEnvironmentByIds$(environmentIds)),
    );
  }

  private getEnvironmentByIds$(environmentIds: number[]): Observable<IEnvironmentDetailed[]> {
    const params = new HttpParams()
      .set('id', environmentIds.join(','))
      .set('fields', environmentDetailedFields);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IEnvironmentDetailed>>(this.environmentsEndpoint, { params })
      .pipe(map(response => response.data.items));
  }
}
