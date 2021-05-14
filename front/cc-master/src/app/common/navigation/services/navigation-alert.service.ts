import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CountResponse } from '@cc/common/queries';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { NavigationAlert } from '../constants/navigation-alert.enum';

@Injectable()
export class NavigationAlertService {

  constructor(private httpClient: HttpClient) {}

  public getAlert$(alert: NavigationAlert): Observable<string> {
    switch (alert) {
      case NavigationAlert.BillingToExport:
        return this.getNotExportedCount$();
      case NavigationAlert.NoAlert:
        return of('');
    }
  }

  private getNotExportedCount$(): Observable<string> {
    const url = 'api/v3/invoices?state.code=1&fields=collection.count';
    return this.httpClient.get<IHttpApiV3CountResponse>(url)
      .pipe(map(r => `${r.data.count}`));
  }
}
