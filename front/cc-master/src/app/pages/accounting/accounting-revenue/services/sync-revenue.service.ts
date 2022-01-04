import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ISyncRevenueInfo } from '../models/sync-revenue-info.interface';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { enIN } from 'date-fns/locale';
import { BillingEntity } from '@cc/domain/billing/clients';
import { map } from 'rxjs/operators';

export class CurrentSyncRevenueInfo {
	syncRevenue: ISyncRevenueInfo;
	entity: BillingEntity; 
}

@Injectable()
export class SyncRevenueService {
  private readonly syncSummaryEndPoint = `/api/v3/contractEntries/syncSummary`;
  private readonly syncEndPoint = `/api/v3/contractEntries/sync`;

  constructor(private httpClient: HttpClient) {}

	public getSyncInfo$(): Observable<CurrentSyncRevenueInfo[]> {
		return this.httpClient.get<IHttpApiV3CollectionResponse<CurrentSyncRevenueInfo>>(this.syncSummaryEndPoint)
			.pipe(map(res => res.data.items));
	}

  public synchronise$(billingEntity: BillingEntity): Observable<void> {
		const body = { billingEntity };

    return this.httpClient.post<void>(this.syncEndPoint, body);
	}
}
