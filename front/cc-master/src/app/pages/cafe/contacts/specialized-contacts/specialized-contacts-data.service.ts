import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DownloadService } from '@cc/common/download';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { AdvancedFilter } from '../../common/cafe-filters/advanced-filter-form';
import { toSearchDto } from '../../common/models';
import { ISpecializedContact } from './specialized-contact.interface';

@Injectable()
export class SpecializedContactsDataService {
  constructor(private httpClient: HttpClient, private downloadService: DownloadService) {
  }

  public getSpecializedContacts$(
    params: HttpParams,
    advancedFilter: AdvancedFilter,
  ): Observable<IHttpApiV4CollectionCountResponse<ISpecializedContact>> {
    params = params.set('fields.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/specialized-contacts/search';
    const url = [route, query].join('?');
    const body = toSearchDto(advancedFilter);

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<ISpecializedContact>>(url, body);
  }

  public exportSpeContacts$(advancedFilter: AdvancedFilter): Observable<void> {
    const route = '/api/cafe/specialized-contacts/export';
    return this.downloadService.download$(route, advancedFilter);
  }
}
