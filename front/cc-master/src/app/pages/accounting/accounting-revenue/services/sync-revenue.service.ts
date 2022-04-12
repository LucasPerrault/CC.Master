import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ISyncRevenueInfo } from '../models/sync-revenue-info.interface';

export class CurrentSyncRevenueInfo {
	syncRevenue: ISyncRevenueInfo;
	entity: string;
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

  public synchronise$(code: string): Observable<void> {
		const body = { billingEntity: code };

    return this.httpClient.post<void>(this.syncEndPoint, body);
	}
}
