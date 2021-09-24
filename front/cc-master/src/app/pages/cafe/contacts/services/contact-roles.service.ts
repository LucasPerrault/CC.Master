import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import { IAppInstance } from '../../instance/models/app-instance.interface';

enum ContactTranslationKey {
  Client = 'cli',
  Specialized = 'spe',
}

@Injectable()
export class ContactRolesService {
  constructor(private translatePipe: TranslatePipe) {}

  public getCommonContactRole(code: string): string {
    // TODO : Add AppContactRole when you will know the code & key for each app.
    return this.getSpeContactRole(code) || this.getClientContactRole(code);
  }

  public getSpeContactRole(code: string): string {
    const translationKey = this.getRoleTranslationKey(ContactTranslationKey.Specialized, code);
    return this.translatePipe.transform(translationKey);
  }

  public getClientContactRole(code: string): string {
    const translationKey = this.getRoleTranslationKey(ContactTranslationKey.Client, code);
    return this.translatePipe.transform(translationKey);
  }

  public getAppContactRole(appInstance: IAppInstance): string {
    return this.translatePipe.transform('cafe_role_app', { name: appInstance?.name });
  }

  private getRoleTranslationKey(key: ContactTranslationKey, code: string): string {
    return `cafe_role_${ key }_${ code }`;
  }
}
