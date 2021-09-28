import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';

import { ContractsRoutingKey } from '../../../../contracts/contracts-manage/services/contracts-routing.service';
import {
  ContactAdditionalColumn,
  IContactAdditionalColumn,
} from '../../common/components/contact-additional-column-select/contact-additional-column.enum';
import { IAppContact } from '../app-contact.interface';

@Component({
  selector: 'cc-app-contact-list',
  templateUrl: './app-contact-list.component.html',
  styleUrls: ['./app-contact-list.component.scss'],
})
export class AppContactListComponent {
  @Input() public contacts: IAppContact[];
  @Input() public selectedColumns: IContactAdditionalColumn[];

  public additionalColumn = ContactAdditionalColumn;

  constructor(private translatePipe: TranslatePipe) { }

  public trackBy(index: number, contact: IAppContact): number {
    return contact.id;
  }

  public getIsConfirmedTranslation(isConfirmed: boolean): string {
    return isConfirmed
      ? this.translatePipe.transform('cafe_contacts_list_isConfirmed_true')
      : this.translatePipe.transform('cafe_contacts_list_isConfirmed_false');
  }

  public isHidden(column: ContactAdditionalColumn): boolean {
    return !this.selectedColumns.find(c => c.id === column);
  }

  public redirectToContracts(environmentId: number): void {
    const query = `${ ContractsRoutingKey.EnvironmentIds }=${ environmentId }`;
    const route = `${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }`;
    const url = [route, query].join('?');

    window.open(url);
  }
}
