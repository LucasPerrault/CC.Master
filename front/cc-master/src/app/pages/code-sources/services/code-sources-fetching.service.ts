import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ICodeSource } from '../models/code-source.interface';

@Injectable()
export class CodeSourcesFetchingService {
  private readonly 	codeSourcesEndpoint = '/api/code-sources';

  constructor(private httpClient: HttpClient) {}

  public getDataFromGithub$(repoUrl: string): Observable<ICodeSource[]> {
    const url = `${ this.codeSourcesEndpoint }/fetch-from-github`;
    return this.httpClient.post<IHttpApiV4CollectionResponse<ICodeSource>>(url, { repoUrl })
      .pipe(map(result => result.items));
  }
}
