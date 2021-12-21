import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { AuthorizationDuration } from './ip-confirm/authorization-duration.interface';

class IpApiRoute {
  public static validity = `/ip-filter/validity`;
  public static confirm = `/ip-filter/confirm`;
  public static reject = `/ip-filter/reject`;
}

export interface IIpRequestValidity {
  expiresAt: string;
}

@Injectable()
export class IpDataService {

  constructor(private httpClient: HttpClient) {}

  public getValidity$(code: string): Observable<IIpRequestValidity> {
    const url = IpApiRoute.validity;
    const params = new HttpParams().set('code', code);

    return this.httpClient.get<IIpRequestValidity>(url, { params });
  }

  public confirm$(code: string, duration: AuthorizationDuration): Observable<void> {
    const url = IpApiRoute.confirm;
    const body = { code, duration };

    return this.httpClient.post<void>(url, body);
  }

  public reject$(code: string): Observable<void> {
    const url = IpApiRoute.reject;
    const body = { code };

    return this.httpClient.post<void>(url, body);
  }
}
