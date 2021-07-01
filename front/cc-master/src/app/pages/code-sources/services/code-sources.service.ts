import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { LifecycleStep } from '../constants/lifecycle-step.enum';
import { ICodeSource } from '../models/code-source.interface';

@Injectable()
export class CodeSourcesService {
  private readonly codeSourcesEndpoint = '/api/code-sources';

  constructor(private httpClient: HttpClient) {}

  public getCodeSources$(): Observable<ICodeSource[]> {
    const params = new HttpParams().set('lifecycle', `${Object.keys(LifecycleStep)}`);

    return this.httpClient.get<IHttpApiV4CollectionResponse<ICodeSource>>(this.codeSourcesEndpoint, { params })
      .pipe(map(response => response.items));
  }

  public getCodeSource$(id: number): Observable<ICodeSource> {
    const url = `${ this.codeSourcesEndpoint }/${ id }`;
    return this.httpClient.get<ICodeSource>(url);
  }

  public getDataFromGithub$(repoUrl: string): Observable<ICodeSource[]> {
    const url = `${ this.codeSourcesEndpoint }/fetch-from-github`;
    return this.httpClient.post<IHttpApiV4CollectionResponse<ICodeSource>>(url, { repoUrl })
      .pipe(map(result => result.items));
  }

  public create$(codeSource: ICodeSource): Observable<void> {
    return this.httpClient.post<void>(this.codeSourcesEndpoint, codeSource);
  }

  public edit$(codeSourceId: number, lifecycle: LifecycleStep): Observable<void> {
    const url = `${ this.codeSourcesEndpoint }/${ codeSourceId }`;
    return this.httpClient.put<void>(url, { lifecycle });
  }
}
