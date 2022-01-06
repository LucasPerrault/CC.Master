import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ApiSortHelper,
  ApiV3DateService,
  IHttpApiV3CollectionResponse,
  IHttpApiV3Response,
} from '@cc/common/queries';
import { startOfMonth } from 'date-fns';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { MiscTransactionInvoiceType } from '../enums/misc-transaction-invoice-type.enum';
import { IMiscellaneousTransaction, miscTransactionFields } from '../models/miscellaneous-transaction.interface';
import { IMiscellaneousTransactionCreationDto } from '../models/miscellaneous-transaction-creation-dto.interface';
import { IMiscellaneousTransactionForm } from '../models/miscellaneous-transaction-form.interface';
import {
  IMiscellaneousTransactionFormContract,
  miscTransactionOfferContractFields,
} from '../models/miscellaneous-transaction-form-contract.interface';

class MiscTransactionEndPoint {
  public static base = '/api/v3/miscellaneousTransactions';
  public static bill = '/api/v3/miscellaneousTransactions/bill';
  public static cancel = (id: number) => `/api/v3/miscellaneousTransactions/${ id }/cancel`;
}

@Injectable()
export class MiscellaneousTransactionsService {
  private readonly contractsEndPoint = '/api/v3/newcontracts';

  constructor(private httpClient: HttpClient, private apiV3DateService: ApiV3DateService) {}

  public getMiscellaneousTransactions$(params: HttpParams): Observable<IMiscellaneousTransaction[]> {
    params = params
      .set('fields', miscTransactionFields)
      .set(ApiSortHelper.v3SortKey, `periodOn,${ ApiSortHelper.v3DscKey },contract.Id,${ ApiSortHelper.v3AscKey }`);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IMiscellaneousTransaction>>(MiscTransactionEndPoint.base, { params })
      .pipe(map(res => res.data.items));
  }

  public getContract$(contractId: number): Observable<IMiscellaneousTransactionFormContract> {
    const url = `${ this.contractsEndPoint }/${ contractId }`;
    const params = new HttpParams()
      .set('fields', miscTransactionOfferContractFields);
    return this.httpClient.get<IHttpApiV3Response<IMiscellaneousTransactionFormContract>>(url, { params })
      .pipe(map(res => res.data));
  }

  public cancelMiscellaneousTransaction$(id: number): Observable<void> {
    return this.httpClient.post<void>(MiscTransactionEndPoint.cancel(id), null);
  }

  public billMiscellaneousTransaction$(ids: number[]): Observable<void> {
    return this.httpClient.post<void>(MiscTransactionEndPoint.bill, { ids });
  }

  public createMiscellaneousTransaction$(form: IMiscellaneousTransactionForm): Observable<void> {
    const body = this.getMiscTransactionCreationDto(form);
    return this.httpClient.post<void>(MiscTransactionEndPoint.base, body);
  }

  private getMiscTransactionCreationDto(form: IMiscellaneousTransactionForm): IMiscellaneousTransactionCreationDto {
    const periodOn = this.apiV3DateService.toApiV3DateFormat(startOfMonth(new Date(form.periodOn)));

    const isCreditNote = form.invoiceType === MiscTransactionInvoiceType.Credit;
    const amount = isCreditNote ? -(form.amount) : form.amount;

    return {
      contractId: form.contract.id,
      comment: form.comment,
      documentLabel: form.documentLabel,
      amount,
      periodOn,
    };
  }
}
