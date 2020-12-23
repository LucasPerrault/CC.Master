import { Pipe, PipeTransform } from '@angular/core';
import { TranslatePipe as NgxTranslatePipe } from '@ngx-translate/core';

@Pipe({ name: 'translate' })
export class TranslatePipe extends NgxTranslatePipe implements PipeTransform {

  transform(queryString: string, ...args: any[]): string {
    const pluralizedKey = this.pluralizeKey(queryString, args);
    return super.transform(pluralizedKey, ...args);
  }

  private pluralizeKey(queryString: string, args: any[]): string {
    if (!args?.length) {
      return queryString;
    }

    const countArg = args.find(arg => !!arg.count || arg.count === 0);
    if (!countArg) {
      return queryString;
    }

    const count = countArg.count;
    const pluralizationFieldName = count === 0 || count === 1
      ? 'one'
      : 'other';

    return `${queryString}.${pluralizationFieldName}`;
  }
}
