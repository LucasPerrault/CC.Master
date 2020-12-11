import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, tap } from 'rxjs/operators';
import { IPrincipal } from './principal.interface';

export function initPrincipal(initializer: PrincipalInitializer): () => Promise<IPrincipal> {
	return () => initializer.initPrincipal();
}
export function getPrincipal(initializer: PrincipalInitializer): IPrincipal {
	return initializer.principal;
}

@Injectable()
export class PrincipalInitializer {
	public principal: IPrincipal;

	constructor(private _http: HttpClient) {}

	public initPrincipal(): Promise<IPrincipal> {
		const fields = 'id,name';
		const principalUrl = `/api/v3/principals/me?fields=${fields}`;

		return this._http.get<{ data: IPrincipal }>(principalUrl).pipe(
			map(res => res.data),
			tap(principal => this.principal = principal,
          err => this.reconnect()
      ),
		).toPromise();
	}

	public reconnect(): void {
		window.location.href = '/account/login?returnUrl=' + encodeURIComponent(window.location.pathname);
	}
}
