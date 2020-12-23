import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
	selector: 'cc-logs',
	templateUrl: './logs.component.html',
})
export class LogsComponent {
  constructor(private httpClient: HttpClient) {
  }
  public getFakeLocaleError(): void {
    throw new Error('Locale Erreur de test');
  }

  public getFakeHttpError(): void {
    this.httpClient.get('api/v3/logs').subscribe();
  }
}
