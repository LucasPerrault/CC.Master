import { Component, OnInit } from '@angular/core';
import { ReplaySubject } from 'rxjs';

import { AdvancedFilter } from './common/cafe-filters/advanced-filters';
import { ICafeConfiguration } from './cafe-configuration.interface';
import { CafeConfiguration } from './cafe-configuration';

@Component({
  selector: 'cc-cafe',
  templateUrl: './cafe.component.html',
})
export class CafeComponent implements OnInit {

  public configuration: ICafeConfiguration;
  public filters$: ReplaySubject<AdvancedFilter> = new ReplaySubject<AdvancedFilter>(1);

  constructor(cafeConfiguration: CafeConfiguration) {
    this.configuration = cafeConfiguration;
  }

  public ngOnInit(): void {
    this.filters$.subscribe(f => console.log('[ADVANCED FILTER]: ', f));
  }

  public updateFilters(filters: AdvancedFilter) {
    this.filters$.next(filters);
  }
}
