import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService } from '@cc/common/queries';
import { IPriceList } from '@cc/domain/billing/offers';
import { startOfMonth } from 'date-fns';
import { Observable } from 'rxjs';

import { IPriceListCreationDto, IPriceRowCreationDto } from '../models/price-list-creation-dto.interface';
import { IPriceListEditionDto, IPriceRowEditionDto } from '../models/price-list-edition-dto.interface';
import { IPriceListForm, IPriceRowForm } from '../models/price-list-form.interface';

class PriceListApiEndpoint {
  public static base = (offerId: number) => `/api/commercial-offers/${ offerId }/price-lists`;
  public static id = (offerId: number, listId: number) => `${ PriceListApiEndpoint.base(offerId) }/${ listId }`;
}

@Injectable()
export class PriceListsDataService {

  constructor(private httpClient: HttpClient, private apiDateService: ApiV3DateService) {
  }

  public create$(offerId: number, form: IPriceListForm): Observable<void> {
    const url = PriceListApiEndpoint.base(offerId);
    const body = this.toCreationDto(form);
    return this.httpClient.post<void>(url, body);
  }

  public edit$(offerId: number, priceList: IPriceList, form: IPriceListForm): Observable<void> {
    const url = PriceListApiEndpoint.id(offerId, priceList.id);
    const body = this.toEditionDto(offerId, priceList, form);
    return this.httpClient.put<void>(url, body);
  }

  public delete$(offerId: number, priceListId: number): Observable<void> {
    const url = PriceListApiEndpoint.id(offerId, priceListId);
    return this.httpClient.delete<void>(url);
  }

  public toCreationDto(priceList: IPriceListForm): IPriceListCreationDto {
    return {
      startsOn: this.apiDateService.toApiV3DateFormat(startOfMonth(new Date(priceList.startsOn))),
      rows: this.toRowsCreationDto(priceList.rows),
    };
  }

  public toEditionDto(offerId: number, priceListToEdit: IPriceList, form: IPriceListForm): IPriceListEditionDto {
    const id = priceListToEdit.id;
    const startsOn = this.apiDateService.toApiV3DateFormat(startOfMonth(new Date(form.startsOn)));
    const rows = this.toRowsEditionDto(id, form.rows);

    return { id, offerId, startsOn, rows };
  }

  private toRowsCreationDto(rows: IPriceRowForm[]): IPriceRowCreationDto[] {
    return rows.map(row => ({
      maxIncludedCount: row.maxIncludedCount,
      unitPrice: row.unitPrice,
      fixedPrice: row.fixedPrice,
    }));
  }

  private toRowsEditionDto(listId: number, rows: IPriceRowForm[]): IPriceRowEditionDto[] {
    const createdRows = rows.filter(row => !row.id);
    const editedRows = rows.filter(row => !!row.id).map(row => ({ ...row, listId }));

    const allRows = [...editedRows, ...this.toRowsCreationDto(createdRows)];
    return allRows.sort((a, b) => a.maxIncludedCount - b.maxIncludedCount);
  }
}
