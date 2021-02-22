import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPrincipal } from '@cc/aspects/principal';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { cloudControlAdmin } from '../constants/user-admin.constant';

@Injectable()
export class UsersService {

  private readonly usersEndPoint = '/api/v3/principals';

  constructor(private httpClient: HttpClient) { }

  public getUsersById$(ids: number[]): Observable<IPrincipal[]> {
    const fields = 'id,name';

    const params = new HttpParams()
      .set('fields', fields)
      .set('id', ids.join(','));

    const users = this.httpClient.get<IHttpApiV3CollectionResponse<IPrincipal>>(this.usersEndPoint, { params })
      .pipe(
        map(response => response.data),
        map(data => data.items),
      );

    return ids.includes(cloudControlAdmin.id)
      ? users.pipe(map(u => [cloudControlAdmin, ...u]))
      : users;
  }
}
