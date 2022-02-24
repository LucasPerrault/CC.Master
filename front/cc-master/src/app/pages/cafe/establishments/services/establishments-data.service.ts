import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DownloadService } from '@cc/common/download';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { AdvancedFilter } from '../../common/components/advanced-filter-form';
import { ISearchDto } from '../../common/models';
import { IEstablishment } from '../../common/models/establishment.interface';

@Injectable()
export class EstablishmentsDataService {
  constructor(private httpClient: HttpClient, private downloadService: DownloadService) {}

  public getEstablishments$(params: HttpParams, searchDto: ISearchDto): Observable<IHttpApiV4CollectionCountResponse<IEstablishment>> {
    params = params.set('fields.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/establishments/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IEstablishment>>(url, searchDto);
  }

  public export$(filter: AdvancedFilter): Observable<void> {
    const route = '/api/cafe/establishments/export';
    return this.downloadService.download$(route, filter);
  }}
