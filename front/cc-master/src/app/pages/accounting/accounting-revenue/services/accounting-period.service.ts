import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService, IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export class CurrentAccountingPeriod {
	date: Date;
	entity: number;
}

class AccountingPeriodsDto {
	date: string;
	entity: number;
}

@Injectable()
export class AccountingPeriodService {
	private readonly accountingPeriodEndPoint = `/api/v3/counts/currentAccountingPeriod`;
	private readonly countsClosePeriodEndPoint = `/api/v3/counts/closePeriod`;

	constructor(private httpClient: HttpClient, private apiV3DateService: ApiV3DateService) { }

	public getCurrentAccountingPeriods$(): Observable<CurrentAccountingPeriod[]> {
		return this.httpClient.get<IHttpApiV3CollectionResponse<AccountingPeriodsDto>>(this.accountingPeriodEndPoint)
			.pipe(map(res => !!res.data ? res.data.items.map(x=> ({ ...x, date: new Date(x.date) })) : null));
	}

	public closePeriod$(period: Date, entity: string): Observable<void> {
		const body = { period: this.apiV3DateService.toApiV3DateFormat(period), billingEntity: entity };
		return this.httpClient.post<void>(this.countsClosePeriodEndPoint, body);
	}
}
