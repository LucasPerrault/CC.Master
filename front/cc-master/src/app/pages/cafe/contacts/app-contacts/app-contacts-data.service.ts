import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DownloadService } from '@cc/common/download';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { AdvancedFilter } from '../../common/components/advanced-filter-form';
import { toSearchDto } from '../../common/models';
import { IAppContact } from './app-contact.interface';

@Injectable()
export class AppContactsDataService {
  constructor(private httpClient: HttpClient, private downloadService: DownloadService) {
  }

  public getAppContacts$(params: HttpParams, advancedFilter: AdvancedFilter): Observable<IHttpApiV4CollectionCountResponse<IAppContact>> {
    params = params.set('fields.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/app-contacts/search';
    const url = [route, query].join('?');
    const body = toSearchDto(advancedFilter);

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IAppContact>>(url, body);
	}

	public exportAppContacts$(advancedFilter: AdvancedFilter): Observable<void> {
    const route = '/api/cafe/app-contacts/export';
    return this.downloadService.download$(route, advancedFilter);
  }
}
