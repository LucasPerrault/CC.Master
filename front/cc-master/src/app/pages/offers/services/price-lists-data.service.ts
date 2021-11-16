import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService } from '@cc/common/queries';
import { startOfMonth } from 'date-fns';
import { Observable } from 'rxjs';

import { IPriceListCreationDto } from '../models/price-list-creation-dto.interface';
import { IPriceListEditionDto } from '../models/price-list-edition-dto.interface';
import { IPriceListForm } from '../models/price-list-form.interface';

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

  public edit$(offerId: number, priceListId: number, form: IPriceListForm): Observable<void> {
    const url = PriceListApiEndpoint.id(offerId, priceListId);
    const body = [this.toEditionDto(priceListId, form)];
    return this.httpClient.put<void>(url, body);
  }

  public delete$(offerId: number, priceListId: number): Observable<void> {
    const url = PriceListApiEndpoint.id(offerId, priceListId);
    return this.httpClient.delete<void>(url);
  }

  public toCreationDto(priceList: IPriceListForm): IPriceListCreationDto {
    return {
      startsOn: this.apiDateService.toApiV3DateFormat(startOfMonth(new Date(priceList.startsOn))),
      rows: priceList.rows,
    };
  }

  private toEditionDto(id: number, form: IPriceListForm): IPriceListEditionDto {
    const startsOn = this.apiDateService.toApiV3DateFormat(startOfMonth(new Date(form.startsOn)));
    const rows = form.rows?.map(row => ({ ...row, listId: id }));
    return { id, startsOn, rows };
  }
}
