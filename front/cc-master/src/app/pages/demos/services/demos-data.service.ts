import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiSortHelper, IHttpApiV4CollectionCountResponse, IHttpApiV4CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { DemosApiRoute } from '../constants/demo-api-route.const';
import { InstancesApiRoute } from '../constants/instance-api-route.const';
import { IDemo, ITemplateDemo } from '../models/demo.interface';
import { IDemoCreationDto, IDemoCreationForm, IDemoDuplication } from '../models/demo-creation-dto.interface';

@Injectable()
export class DemosDataService {
  constructor(private httpClient: HttpClient) {}

  public getDemos$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<IDemo>> {
    params = params
      .set('fields.root', 'count')
      .set(ApiSortHelper.v4SortKey, `${ ApiSortHelper.v4DscKey }createdAt`);

    return this.httpClient.get<IHttpApiV4CollectionCountResponse<IDemo>>(DemosApiRoute.base, { params });
  }

  public getDemo$(id: number): Observable<IDemo> {
    return this.httpClient.get<IDemo>(DemosApiRoute.id(id));
  }

  public getTemplateDemos$(): Observable<ITemplateDemo[]> {
    const params = new HttpParams().set('isTemplate', true.toString());
    return this.httpClient.get<IHttpApiV4CollectionResponse<ITemplateDemo>>(DemosApiRoute.base, { params })
      .pipe(map(res => res.items));
  }

  public getDefaultTemplateDemo$(): Observable<ITemplateDemo> {
    const masterDemoId = 385;
    return this.httpClient.get<ITemplateDemo>(DemosApiRoute.id(masterDemoId));
  }

  public create$(form: IDemoCreationForm): Observable<IDemoDuplication> {
    const body = this.getCreationDto(form);
    return this.httpClient.post<IDemoDuplication>(DemosApiRoute.duplicate, body);
  }

  public delete$(id: number): Observable<void> {
    return this.httpClient.delete<void>(DemosApiRoute.id(id));
  }

  public editComment$(id: number, comment: string): Observable<void> {
    return this.httpClient.put<void>(DemosApiRoute.id(id), { comment });
  }

  public editPassword$(instanceId: number, password: string): Observable<void> {
    const url = InstancesApiRoute.editPassword(instanceId, password);
    return this.httpClient.post<void>(url, {});
  }

  public protect$(instanceId: number, isProtected: boolean): Observable<void> {
    const url = isProtected ? InstancesApiRoute.lock(instanceId) : InstancesApiRoute.unlock(instanceId);
    return this.httpClient.put<void>(url, {});
  }

  private getCreationDto(form: IDemoCreationForm): IDemoCreationDto {
    return {
      subdomain: form?.subdomain?.toLowerCase(),
      sourceId: form?.source?.id,
      password: form?.password,
      distributorCode: form?.distributor?.code ?? 'LUC', // PB: We don't have principal.departmentCode ?
      comment: form?.comment ?? '',
    };
  }
}
