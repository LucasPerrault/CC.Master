import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService, IHttpApiV3Response } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class AccountingPeriodService {
  private readonly accountingPeriodEndPoint = `/api/v3/counts/currentAccountingPeriod`;
  private readonly countsClosePeriodEndPoint = `/api/v3/counts/closePeriod`;

  constructor(private httpClient: HttpClient, private apiV3DateService: ApiV3DateService) {}

	public getCurrentAccountingPeriod$(): Observable<Date> {
		return this.httpClient.get<IHttpApiV3Response<string>>(this.accountingPeriodEndPoint)
			.pipe(map(res => !!res.data ? new Date(res.data) : null));
	}

  public closePeriod$(period: Date): Observable<void> {
    const body = { period: this.apiV3DateService.toApiV3DateFormat(period) };
    return this.httpClient.post<void>(this.countsClosePeriodEndPoint, body);
  }
}
