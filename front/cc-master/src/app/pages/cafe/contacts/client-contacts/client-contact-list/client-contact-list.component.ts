import { Component, Input } from '@angular/core';

import {
  IContactAdditionalColumn,
} from '../../common/components/contact-additional-column-select/contact-additional-column.interface';
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

  constructor() { }

  public trackBy(index: number, contact: IClientContact): number {
    return contact.id;
  }

  public getEnvironmentName(subdomain: string, domain: string): string {
    return `${ subdomain }.${ domain }`;
  }

  public isHidden(column: ClientContactAdditionalColumn): boolean {
    return !this.selectedColumns.find(c => c.id === column);
  }
}
