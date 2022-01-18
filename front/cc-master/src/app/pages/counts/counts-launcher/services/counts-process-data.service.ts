import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IDateRange } from '@cc/common/date';
import { ApiV3DateService, IHttpApiV3CountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class CountsProcessDataService {

  constructor(private httpClient: HttpClient, private apiDateService: ApiV3DateService) {}

  public launchCountProcess$(period: IDateRange): Observable<void> {
    const url = '/api/v3/countProcess';
    const from = this.apiDateService.toApiV3DateFormat(period.startDate);
    const to = this.apiDateService.toApiV3DateFormat(period.endDate);
    const body = { from, to };

    return this.httpClient.post<void>(url, body);
  }

  public getCountProcessNumber$(): Observable<number> {
    const url = '/api/v3/countProcess';
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('stateId', '0');

    return this.httpClient.get<IHttpApiV3CountResponse>(url, { params })
      .pipe(map(res => res.data.count));
  }
}
