import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { LifecycleStep } from '../constants/lifecycle-step.enum';
import { ICodeSource } from '../models/code-source.interface';

@Injectable()
export class CodeSourcesListService {
  private readonly codeSourcesEndpoint = '/api/code-sources';

  constructor(private httpClient: HttpClient) {}

  public getCodeSources$(): Observable<ICodeSource[]> {
    const params = new HttpParams()
      .set('lifecycle', `${Object.keys(LifecycleStep)}`)
      .set('orderBy', 'code,asc');

    return this.httpClient.get<IHttpApiV4CollectionResponse<ICodeSource>>(this.codeSourcesEndpoint, { params })
      .pipe(map(response => response.items));
  }

  public getActiveCodeSources(sources: ICodeSource[]): ICodeSource[] {
    return sources.filter(cs => cs.lifecycle === LifecycleStep.Preview
      || cs.lifecycle === LifecycleStep.ReadyForDeploy
      || cs.lifecycle === LifecycleStep.InProduction,
    );
  }

  public getReferencedCodeSources(sources: ICodeSource[]): ICodeSource[] {
    return sources.filter(cs => cs.lifecycle === LifecycleStep.Referenced);
  }

  public getDeletedCodeSources(sources: ICodeSource[]): ICodeSource[] {
    return sources.filter(cs => cs.lifecycle === LifecycleStep.ToDelete || cs.lifecycle === LifecycleStep.Deleted);
  }
}
