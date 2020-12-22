import { TranslateLoader as NgxTranslateLoader } from '@ngx-translate/core';
import { Observable, of } from 'rxjs';

import { Translations } from '../../../translations/translations';

export class TranslateLoader implements NgxTranslateLoader {

  public getTranslation(lang: string): Observable<any> {
    return of(Translations[lang] || {});
  }
}
