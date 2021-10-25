import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  IContactAdditionalColumn,
} from '../../common/components/contact-additional-column-select/contact-additional-column.interface';
import { IAppContact } from '../app-contact.interface';
import { AppContactAdditionalColumn } from '../app-contact-additional-column.enum';

@Component({
  selector: 'cc-app-contact-list',
  templateUrl: './app-contact-list.component.html',
  styleUrls: ['./app-contact-list.component.scss'],
})
export class AppContactListComponent {
  @Input() public contacts: IAppContact[];
  @Input() public selectedColumns: IContactAdditionalColumn[];

  public additionalColumn = AppContactAdditionalColumn;

  constructor(private translatePipe: TranslatePipe) { }

  public trackBy(index: number, contact: IAppContact): number {
    return contact.id;
  }

  public getEnvironmentName(subdomain: string, domain: string): string {
    return `${ subdomain }.${ domain }`;
  }

  public getIsConfirmedTranslation(isConfirmed: boolean): string {
    return isConfirmed
      ? this.translatePipe.transform('cafe_contacts_list_isConfirmed_true')
      : this.translatePipe.transform('cafe_contacts_list_isConfirmed_false');
  }

  public isHidden(column: AppContactAdditionalColumn): boolean {
    return !this.selectedColumns.find(c => c.id === column);
  }
}
