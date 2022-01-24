import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { cloudControlAdmin } from '@cc/domain/users';
import { Observable } from 'rxjs';

import { InstancesApiRoute } from '../constants/instance-api-route.const';

@Injectable()
export class ConnectAsDataService {
	constructor(private http: HttpClient) {}

	public getConnectionUrl$(userId: number, instanceId: number): Observable<string> {
		return this.http.post<string>(InstancesApiRoute.connectAs(instanceId), { userId });
	}

	public getConnectionUrlAsMasterKey$(instanceId: number): Observable<string> {
		return this.getConnectionUrl$(cloudControlAdmin.id, instanceId);
	}
}
