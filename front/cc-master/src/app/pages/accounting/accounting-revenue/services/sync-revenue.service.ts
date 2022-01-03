import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ISyncRevenueInfo } from '../models/sync-revenue-info.interface';
import { BillingEntity } from '@cc/domain/billing/billing-entity';
import { map } from 'rxjs/operators';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { enIN } from 'date-fns/locale';

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
			.pipe(map(res => !!res.data ? res.data.items.map(x=> ({...x, syncRevenue: x.syncRevenue})) : null));
	}

  public synchronise$(entity: BillingEntity): Observable<void> {
		const body = { billingEntity: this.getEnumKeyByEnumValue(BillingEntity,entity) };

    return this.httpClient.post<void>(this.syncEndPoint, body);
	}
	
	public getEnumKeyByEnumValue(myEnum, enumValue) {
    let keys = Object.keys(myEnum).filter(x => myEnum[x] == enumValue);
    return keys.length > 0 ? keys[0] : null;
	}
}
