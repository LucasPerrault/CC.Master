import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { IHttpApiV3Response } from '@cc/common/queries';

@Injectable()
export class AccountingPeriodService {
  private readonly accountingPeriodEndPoint = `/api/v3/counts/currentAccountingPeriod`;

  constructor(private httpClient: HttpClient) {}

	public getCurrentAccountingPeriod$(): Observable<Date> {
		return this.httpClient.get<IHttpApiV3Response<string>>(this.accountingPeriodEndPoint)
			.pipe(map(res => !!res.data ? new Date(res.data) : null));
	}
}
