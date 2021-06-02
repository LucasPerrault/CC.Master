import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';

@Injectable()
export class CookiesService {
  private readonly isDocumentAccessible: boolean;
  private readonly authTokenKey = 'authToken';

  private readonly expirationDateKeyword = 'expires';

  // eslint-disable-next-line @typescript-eslint/ban-types
  constructor(@Inject(PLATFORM_ID) private platformId: Object, @Inject(DOCUMENT) private document: Document) {
    this.isDocumentAccessible = isPlatformBrowser(platformId);
  }

  public deleteAuthToken(): void {
    this.delete(this.authTokenKey);
  }

  private delete(key: string): void {
    if (!this.isDocumentAccessible) {
      return;
    }

    const expiredDate = new Date(0);
    this.setCookie(key, '', expiredDate);
  }

  private setCookie(key: string, value: string, expiresDate?: Date): void {
    if (!this.isDocumentAccessible) {
      return;
    }

    let cookiesString: string = encodeURIComponent(key) + '=' + encodeURIComponent(value) + ';';

    if (!!expiresDate) {
      cookiesString += `${ this.expirationDateKeyword }=` + expiresDate.toUTCString() + ';';
    }

    this.document.cookie = cookiesString;
  }
}
