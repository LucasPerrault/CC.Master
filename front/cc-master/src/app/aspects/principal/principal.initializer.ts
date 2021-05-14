import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, tap } from 'rxjs/operators';

import { IPrincipal } from './principal.interface';

export const initPrincipal = (initializer: PrincipalInitializer): () => Promise<IPrincipal> => () => initializer.initPrincipal();
export const getPrincipal = (initializer: PrincipalInitializer): IPrincipal => initializer.principal;
export const getCultureCode = (initializer: PrincipalInitializer): string => initializer.principal.culture.code;

@Injectable()
export class PrincipalInitializer {
	public principal: IPrincipal;

	constructor(private http: HttpClient) {}

	public initPrincipal(): Promise<IPrincipal> {
		const fields = 'id,name,isLuccaUser,permissions[scope,operation[id]],culture[code]';
		const principalUrl = `/api/v3/principals/me?fields=${fields}`;

		return this.http.get<{ data: IPrincipal }>(principalUrl).pipe(
			map(res => res.data),
			tap(principal => this.principal = principal,
          err => this.reconnect(),
      ),
		).toPromise();
	}

	public reconnect(): void {
		window.location.href = '/account/login?returnUrl=' + encodeURIComponent(window.location.pathname);
	}
}
