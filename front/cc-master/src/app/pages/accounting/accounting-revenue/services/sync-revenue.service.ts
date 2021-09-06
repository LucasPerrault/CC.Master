import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ISyncRevenueInfo } from '../models/sync-revenue-info.interface';

@Injectable()
export class SyncRevenueService {
  private readonly syncSummaryEndPoint = `/api/v3/contractEntries/syncSummary`;
  private readonly syncEndPoint = `/api/v3/contractEntries/sync`;

  constructor(private httpClient: HttpClient) {}

	public getSyncInfo$(): Observable<ISyncRevenueInfo> {
		return this.httpClient.get<ISyncRevenueInfo>(this.syncSummaryEndPoint);
	}

  public synchronise$(): Observable<void> {
    return this.httpClient.post<void>(this.syncEndPoint, null);
  }
}
