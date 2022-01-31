import { Component, Input } from '@angular/core';

import { IEstablishment } from '../../../common/models/establishment.interface';

@Component({
  selector: 'cc-establishment-list',
  templateUrl: './establishment-list.component.html',
  styleUrls: ['./establishment-list.component.scss'],
})
export class EstablishmentListComponent {
  @Input() public establishments: IEstablishment[];

  public trackBy(index: number, establishment: IEstablishment): number {
    return establishment.id;
  }

  public getEnvironmentName(subdomain: string, domain: string): string {
    return `${ subdomain }.${ domain }`;
  }
}
