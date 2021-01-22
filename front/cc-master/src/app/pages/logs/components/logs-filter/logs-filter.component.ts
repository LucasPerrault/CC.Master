import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';
import { FiltersService, IFilterParams } from '@cc/common/filters';

import { EnvironmentLogFilterKeyEnum } from '../../enums';

@Component({
  selector: 'cc-logs-filter',
  templateUrl: './logs-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsFiltersComponent {
  @Output() public updateFilters: EventEmitter<IFilterParams> = new EventEmitter<IFilterParams>();

  public logsFilterParamsKeys = EnvironmentLogFilterKeyEnum;

  private filters: IFilterParams;

  constructor(private filterService: FiltersService) {
  }

  public update(key: string, value: string): void {
    const filtersUpdated = this.filterService.updateParams(this.filters, key, value);
    this.updateFilters.emit(filtersUpdated);
  }
}
