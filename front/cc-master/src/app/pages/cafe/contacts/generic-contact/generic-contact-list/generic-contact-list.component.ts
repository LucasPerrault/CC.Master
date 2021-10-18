import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';

import { ContractsRoutingKey } from '../../../../contracts/contracts-manage/services/contracts-routing.service';
import {
  IContactAdditionalColumn,
} from '../../common/components/contact-additional-column-select/contact-additional-column.interface';
import { ContactRolesService } from '../../common/services/contact-roles.service';
import { IGenericContact } from '../generic-contact.interface';
import { GenericContactAdditionalColumn } from '../generic-contact-additional-column.enum';

@Component({
  selector: 'cc-generic-contacts-list',
  templateUrl: './generic-contact-list.component.html',
  styleUrls: ['./generic-contact-list.component.scss'],
})
export class GenericContactListComponent {
  @Input() public contacts: IGenericContact[];
  @Input() public selectedColumns: IContactAdditionalColumn[];

  public additionalColumn = GenericContactAdditionalColumn;

  constructor(
    private rolesService: ContactRolesService,
    private translatePipe: TranslatePipe,
  ) { }

  public trackBy(index: number, contact: IGenericContact): number {
    return contact.id;
  }

  public getRole(code: string): string {
    return this.rolesService.getGenericContactRole(code);
  }

  public getIsConfirmedTranslation(isConfirmed: boolean): string {
    return isConfirmed
      ? this.translatePipe.transform('cafe_contacts_list_isConfirmed_true')
      : this.translatePipe.transform('cafe_contacts_list_isConfirmed_false');
  }

  public isHidden(column: GenericContactAdditionalColumn): boolean {
    return !this.selectedColumns.find(c => c.id === column);
  }

  public redirectToContracts(environmentId: number): void {
    const query = `${ ContractsRoutingKey.EnvironmentIds }=${ environmentId }`;
    const route = `${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }`;
    const url = [route, query].join('?');

    window.open(url);
  }
}
