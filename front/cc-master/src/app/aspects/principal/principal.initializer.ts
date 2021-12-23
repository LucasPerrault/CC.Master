import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';

import { IPrincipal } from './principal.interface';

export const initPrincipal = (initializer: PrincipalInitializer): () => Promise<IPrincipal> => () => initializer.initPrincipal();
export const getPrincipal = (initializer: PrincipalInitializer): IPrincipal => initializer.principal;
export const getCultureCode = (initializer: PrincipalInitializer): string => initializer.principal.culture.code;

@Injectable()
export class PrincipalInitializer {
  public principal: IPrincipal;

  constructor(private http: HttpClient) {}

  public initPrincipal(): Promise<IPrincipal> {
    const principalUrl = `/api/principals/me`;

    return this.http.get<IPrincipal>(principalUrl).pipe(
      tap(principal => this.principal = principal,
        err => this.reconnect(),
      ),
    ).toPromise();
  }

  public reconnect(): void {
    window.location.href = '/account/login?returnUrl=' + encodeURIComponent(window.location.pathname);
  }
}
