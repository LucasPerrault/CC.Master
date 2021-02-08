import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { apiDateBetweenKeyword, apiDateFormat, apiDateSinceKeyword, apiDateUntilKeyword } from '@cc/common/queries';
import { LuNativeDateAdapter } from '@lucca-front/ng/core';

@Component({
  selector: 'cc-date-range-select',
  templateUrl: './date-range-select.component.html',
})
export class DateRangeSelectComponent implements OnInit {
  @Output() public dateRangeToString: EventEmitter<string> = new EventEmitter<string>();

  public startDate;
  public endDate;
  public todayDate = new Date();

  private routerParamKey = 'date';

  constructor(private adapter: LuNativeDateAdapter, private router: Router, private activatedRoute: ActivatedRoute) {
  }

  public ngOnInit() {
    const routerParamValue = this.activatedRoute.snapshot.queryParamMap.get(this.routerParamKey);
    if (!routerParamValue) {
      return;
    }

    this.dateRangeToString.emit(routerParamValue);
    this.setDatesWithQueryString(routerParamValue.split(','));
  }

  public async updateDatesSelectedAsync(startDate: Date, endDate: Date): Promise<void> {
    const dateRangeToApiQueryString = this.getDateRangeToQueryString(startDate, endDate);
    this.dateRangeToString.emit(dateRangeToApiQueryString);

    await this.updateRouterAsync(dateRangeToApiQueryString);
  }

  private setDatesWithQueryString(queryParams: string[]): void {
    const dateKeyword = queryParams.shift();
    if (!dateKeyword) {
      return;
    }

    if (dateKeyword === apiDateBetweenKeyword) {
      this.startDate = new Date(queryParams.shift());
      this.endDate = new Date(queryParams.shift());
      return;
    }

    if (dateKeyword === apiDateSinceKeyword) {
      this.startDate = new Date(queryParams.shift());
      return;
    }

    if (dateKeyword === apiDateUntilKeyword) {
      this.endDate = new Date(queryParams.shift());
    }
  }

  private getDateRangeToQueryString(startDate: Date, endDate: Date): string {
    if (!startDate && !endDate) {
      return;
    }

    const startDateFormat = this.getDateToApiQueryString(startDate);
    const endDateFormat = this.getDateToApiQueryString(endDate);

    if (!endDateFormat) {
      return `${apiDateSinceKeyword},${startDateFormat}`;
    }
    if (!startDateFormat) {
      return `${apiDateUntilKeyword},${endDateFormat}`;
    }

    return `${apiDateBetweenKeyword},${startDateFormat},${endDateFormat}`;
  }

  private getDateToApiQueryString(date: Date): string {
    if (!date) {
      return;
    }

    return this.adapter.format(date, apiDateFormat);
  }

  private async updateRouterAsync(value: string): Promise<void> {
    const queryParams = { [this.routerParamKey]: !!value ? value : null };

    await this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams,
      queryParamsHandling: 'merge',
    });
  }
}
