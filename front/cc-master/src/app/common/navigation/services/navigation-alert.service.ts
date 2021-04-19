import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class NavigationAlertService {

  constructor(private httpClient: HttpClient) {}

  public getNotExportedCount$(): Observable<string> {
    const url = 'api/v3/invoices?state.code=1&fields=collection.count';
    return this.httpClient.get<IHttpApiV3CountResponse>(url)
      .pipe(map(r => `${r.data.count}`));
  }
}
