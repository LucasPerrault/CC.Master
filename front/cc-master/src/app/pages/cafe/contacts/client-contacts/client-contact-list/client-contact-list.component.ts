import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';

import { ContractsRoutingKey } from '../../../../contracts/contracts-manage/services/contracts-routing.service';
import {
  IContactAdditionalColumn,
} from '../../common/components/contact-additional-column-select/contact-additional-column.interface';
import { ContactRolesService } from '../../common/services/contact-roles.service';
import { IClientContact } from '../client-contact.interface';
import { ClientContactAdditionalColumn } from '../client-contact-additional-column.enum';

@Component({
  selector: 'cc-client-contact-list',
  templateUrl: './client-contact-list.component.html',
  styleUrls: ['./client-contact-list.component.scss'],
})
export class ClientContactListComponent {
  @Input() public contacts: IClientContact[];
  @Input() public selectedColumns: IContactAdditionalColumn[];

  public additionalColumn = ClientContactAdditionalColumn;

  constructor(
    private translatePipe: TranslatePipe,
    private rolesService: ContactRolesService,
  ) { }

  public trackBy(index: number, contact: IClientContact): number {
    return contact.id;
  }

  public getIsConfirmedTranslation(isConfirmed: boolean): string {
    return isConfirmed
      ? this.translatePipe.transform('cafe_contacts_list_isConfirmed_true')
      : this.translatePipe.transform('cafe_contacts_list_isConfirmed_false');
  }

  public getRole(code: string): string {
    return this.rolesService.getClientContactRole(code);
  }

  public isHidden(column: ClientContactAdditionalColumn): boolean {
    return !this.selectedColumns.find(c => c.id === column);
  }

  public redirectToContracts(environmentId: number): void {
    const query = `${ ContractsRoutingKey.EnvironmentIds }=${ environmentId }`;
    const route = `${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }`;
    const url = [route, query].join('?');

    window.open(url);
  }
}
