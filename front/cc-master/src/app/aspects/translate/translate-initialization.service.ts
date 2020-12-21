import {Injectable} from '@angular/core';
import {TranslateService} from '@ngx-translate/core';

@Injectable()
export class TranslateInitializationService {

  private defaultLanguageKey = 'fr';

  constructor(private translateService: TranslateService) {
  }

  public initializeTranslations(localeId: string): void {
    this.translateService.setDefaultLang(this.defaultLanguageKey);
    const languageKey = this.getLanguageKey(localeId);
    this.translateService.use(languageKey);
  }

  private getLanguageKey(localeId: string): string {
    return localeId.substring(0, 2);
  }
}
