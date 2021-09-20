import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';


@Injectable()
export class DownloadService {
  constructor(private httpClient: HttpClient) {}

  public download$(href: string, body = null): Observable<void> {
    const httpResponse$ = this.httpClient.post(href, body, {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
      responseType: 'blob',
      observe: 'response',
    });

    return httpResponse$.pipe(
      map((response: HttpResponse<Blob>) => {
        const filename = this.getFilename(response.headers);
        this.download(filename, response.body);
      }),
    );
  }

  private getFilename(headers: HttpHeaders): string {
    const content = headers.get('content-disposition');
    const filenamePart = content.split(';')[1];
    return filenamePart.replace('filename=', '').trim();
  }


  private download(fileName: string, blob?: Blob): void {
    const url = window.URL.createObjectURL(blob);

    const element = document.createElement('a');
    element.setAttribute('href', url);
    element.setAttribute('download', fileName);

    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);

    window.URL.revokeObjectURL(url);
  }
}
