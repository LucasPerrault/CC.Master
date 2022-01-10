import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import { IAppInstance } from '../../models/app-instance.interface';
import { DistributorType, IEnvironment } from '../../models/environment.interface';
import { ILegalUnit } from '../../models/legal-unit.interface';
import {
  EnvironmentAdditionalColumn,
  IEnvironmentAdditionalColumn,
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

  constructor(private translatePipe: TranslatePipe)
  { }

  public trackBy(index: number, environment: IEnvironment): number {
    return environment.id;
  }

  public getEnvironmentName(subdomain: string, domain: string): string {
    return `${ subdomain }.${ domain }`;
  }

  public getAppInstanceNames(appInstances: IAppInstance[]): string {
    return appInstances.map(app => app.applicationName).join(', ');
  }

  public getCountriesCount(legalUnits: ILegalUnit[]): number {
    return legalUnits.map(l => l.country).length;
  }

  public getCountryNames(legalUnits: ILegalUnit[]): string {
    const countries = legalUnits.map(l => l.country.name) ?? [];
    const distinctCountries = countries.filter((value, index, self) => self.indexOf(value) === index);
    const sortedCountries = distinctCountries.sort((a, b) => a.localeCompare(b));
    return sortedCountries.join(', ');
  }

  public getDistributorNames(environment: IEnvironment): string {
    const distributors = environment.accesses?.map(a => a.distributor) ?? [];
    const distributorsWithoutLucca = distributors?.filter(d => !d.isLucca);
    const luccaDistributor = distributors?.find(d => d.isLucca);

    const names = distributorsWithoutLucca?.map(a => a.name);
    const distinct = names.filter((value, index, self) => self.indexOf(value) === index);
    const sorted = distinct.sort((a, b) => a.localeCompare(b));

    const distributorNames = !!luccaDistributor ? [luccaDistributor?.name, ...sorted] : sorted;
    return distributorNames.join(', ');
  }

  public getDistributorTypeTranslation(environment: IEnvironment): string {
    switch (environment.distributorType) {
      case DistributorType.DirectOnly:
        return this.translatePipe.transform('cafe_environments_list_direct_only');
      case DistributorType.IndirectOnly:
        return this.translatePipe.transform('cafe_environments_list_indirect_only');
      case DistributorType.DirectAndIndirect:
        return this.translatePipe.transform('cafe_environments_list_direct_and_indirect');
    }
  }

  public isHidden(column: EnvironmentAdditionalColumn): boolean {
    return !this.selectedColumns.find(c => c.id === column);
  }
}
