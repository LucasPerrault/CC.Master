import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IInstanceDuplication } from '@cc/domain/instances';
import { Observable } from 'rxjs';

class InstanceDuplicationRoute {
  public static base = '/api/instanceduplications';
  public static id = (id: string) => `${ InstanceDuplicationRoute.base }/${ id }`;
}

@Injectable()
export class InstancesDuplicationsDataService {

  constructor(private httpClient: HttpClient) {}

  public getDuplication$(id: string): Observable<IInstanceDuplication> {
    return this.httpClient.get<IInstanceDuplication>(InstanceDuplicationRoute.id(id));
  }
}
