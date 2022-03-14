import { Injectable } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CalendlyEmbedType, CalendlyQueryMapping, ICalendlyPrefill } from '@cc/common/calendly/model/calendly-format-url.interface';
import { IAccountManager } from '@cc/common/calendly/services/account-managers-data.service';

@Injectable()
export class CalendlyService {
  private readonly calendlyUrl = 'https://calendly.com';

  constructor(private sanitizer: DomSanitizer) {}

  public getUrl(accountManager: IAccountManager, embedType?: CalendlyEmbedType, prefill?: ICalendlyPrefill): SafeResourceUrl {
    const baseUrl = this.getBaseUrl(accountManager?.email);
    const httpParams = CalendlyQueryMapping.toHttpParams(embedType, prefill);
    const url = [baseUrl, httpParams.toString()].join('?');
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }


  private getBaseUrl(email: string): string {
    return `${ this.calendlyUrl }/${ email.slice(0, email.indexOf('@')) }`;
  }

}
