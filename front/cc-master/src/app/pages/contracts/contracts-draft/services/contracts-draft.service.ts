import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import {
  ApiSortHelper,
  IHttpApiV3CollectionCount,
  IHttpApiV3CollectionResponse,
  IHttpApiV3Response,
} from '@cc/common/queries';
import { IContract, IContractForm } from '@cc/domain/billing/contracts';
import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import {
  draftFields as minDraftFields,
  draftFormFields,
  draftListEntryFields,
  IContractConversionDto,
  IContractDeletionDto,
  IContractDraft,
  IContractDraftForm,
  IContractDraftListEntry,
} from '../models';

const opportunityFields = (draftFields: string) => [
  'id',
  'name',
  'isDirectSales',
  'urlToSFOpportunity',
  'urlToSFDistributor',
  'closeOn',
  `contractDrafts[${ draftFields }]`,
].join(',');

interface IOpportunity<T extends IContractDraft> {
  id: number;
  name: string;
  isDirectSales: boolean;
  urlToSFOpportunity: string;
  urlToSFDistributor: string;
  closeOn: string;
  contractDrafts: T[];
}

@Injectable()
export class ContractsDraftService {
  private readonly opportunitiesEndPoint = '/api/v3/opportunities';

  constructor(private httpClient: HttpClient, private translatePipe: TranslatePipe) {
  }

  public deleteContractDraft$(draftId: string): Observable<void> {
    const url$ = this.getOpportunityId$(draftId).pipe(
      map(opportunityId => `${ this.opportunitiesEndPoint }/${ opportunityId }/abort`),
    );

    const body = this.getDraftDeleteBody(draftId);
    return url$.pipe(switchMap(url => this.httpClient.post<void>(url, body)));
  }

  public convertToContract$(draftId: string, form: IContractForm): Observable<IContract> {
    const body = this.getContractConvertBody(draftId, form);

    const url$ = this.getOpportunityId$(draftId).pipe(
      map(opportunityId => `${ this.opportunitiesEndPoint }/${ opportunityId }/convert`),
    );

    return url$.pipe(switchMap(url => this.httpClient.post<IContract>(url, body)));
  }

  public getContractDrafts$(httpParams: HttpParams): Observable<IHttpApiV3CollectionCount<IContractDraftListEntry>> {
    return this.getOpportunities$<IContractDraftListEntry>(httpParams, draftListEntryFields).pipe(
      this.toContractDrafts(),
      map(drafts => ({
        items: drafts,
        count: drafts.length,
      })),
    );
  }

  public getContractDraft$(id: string): Observable<IContractDraftForm> {
    const opportunity = this.getOpportunityId$(id).pipe(
      switchMap((opportunityId: number) => this.getOpportunity$<IContractDraftForm>(opportunityId, draftFormFields)),
    );

    return opportunity.pipe(
      map(o => this.getContractDrafts<IContractDraftForm>(o).find(d => d.id === id)),
    );
  }

  private getOpportunityId$(draftId: string): Observable<number> {
    const filterByDraftId = new HttpParams().set('contractDrafts.opportunityLineItemReccuringId', draftId);

    return this.getOpportunities$<IContractDraft>(filterByDraftId, minDraftFields).pipe(
      map(opportunities => {
        if (!opportunities.length) {
          const message = this.translatePipe.transform('front_draftPage_getOpportunityId_notFoundError', { draftId });
          throw new Error(message);
        }

        return opportunities[0].id;
      }),
    );
  }

  private getOpportunities$<T extends IContractDraft>(httpParams: HttpParams, draftFields: string): Observable<IOpportunity<T>[]> {
    const params = httpParams.set('fields', opportunityFields(draftFields))
      .set(ApiSortHelper.v3SortKey, `name,${ApiSortHelper.v3AscKey}`);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IOpportunity<T>>>(this.opportunitiesEndPoint, { params })
      .pipe(map(response => response.data.items));
  }

  private getOpportunity$<T extends IContractDraft>(id: number, contractDraftFields: string): Observable<IOpportunity<T>> {
    const urlById = `${ this.opportunitiesEndPoint }/${ id }`;
    const params = new HttpParams().set('fields', opportunityFields(contractDraftFields));

    return this.httpClient.get<IHttpApiV3Response<IOpportunity<T>>>(urlById, { params })
      .pipe(map(response => response.data));
  }

  private toContractDrafts(): UnaryFunction<Observable<IOpportunity<IContractDraftListEntry>[]>, Observable<IContractDraftListEntry[]>> {
    return pipe(map(opportunities =>
      opportunities.reduce((acc, o) =>
        acc.concat(this.getContractDrafts(o)), [],
      ),
    ));
  }

  private getContractDrafts<T extends IContractDraft>(opportunity: IOpportunity<T>): T[] {
    return opportunity.contractDrafts.map(draft => ({
      id: draft.opportunityLineItemReccuringId,
      createdAt: opportunity.closeOn,
      externalUrl: opportunity.urlToSFOpportunity,
      externalDistributorUrl: opportunity.urlToSFDistributor,
      isDirectSales: opportunity.isDirectSales,
      ...draft,
    }));
  }

  private getDraftDeleteBody(draftId: string): IContractDeletionDto {
    return {
      contractDrafts: [{
        opportunityLineItemReccuringId: draftId,
      }],
    };
  }

  private getContractConvertBody(draftId: string, contractForm: IContractForm): IContractConversionDto {
    return {
      contractDrafts: [{
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
        opportunityLineItemReccuringId: draftId,
      }],
    };
  }
}
