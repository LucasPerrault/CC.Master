import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { LuModal } from '@lucca-front/ng/modal';

import { IAppInstance } from '../../models/app-instance.interface';
import { IEnvironment } from '../../models/environment.interface';
import { ILegalUnit } from '../../models/legal-unit.interface';
import { CountryListModalComponent } from '../country-list-modal/country-list-modal.component';
import {
  EnvironmentAdditionalColumn,
  IEnvironmentAdditionalColumn
} from '../environment-additional-column-select/environment-additional-column.enum';

@Component({
  selector: 'cc-environment-list',
  templateUrl: './environment-list.component.html',
  styleUrls: ['./environment-list.component.scss'],
})
export class EnvironmentListComponent {
  @Input() public environments: IEnvironment[];
  @Input() public selectedColumns: IEnvironmentAdditionalColumn[];

  public additionalColumn = EnvironmentAdditionalColumn;

  constructor(private translatePipe: TranslatePipe, private luModal: LuModal) { }

  public trackBy(index: number, environment: IEnvironment): number {
    return environment.id;
  }

  public getAppInstanceNames(appInstances: IAppInstance[]): string {
    return appInstances.map(app => app.name).join(', ');
  }

  public getCountriesCount(legalUnits: ILegalUnit[]): number {
    return legalUnits.map(l => l.country).length;
  }

  public openCountryListModal(legalUnits: ILegalUnit[]): void {
    this.luModal.open(CountryListModalComponent);
  }

  public getTranslatedIsArchived(isActive: boolean): string {
    return isActive
      ? this.translatePipe.transform('cafe_list_isArchived_false')
      : this.translatePipe.transform('cafe_list_isArchived_true');
  }

  public isHidden(column: EnvironmentAdditionalColumn): boolean {
    return !this.selectedColumns.find(c => c.id === column);
  }

  public getDistributorNames(environment: IEnvironment): string {
    return '';
  }
}
