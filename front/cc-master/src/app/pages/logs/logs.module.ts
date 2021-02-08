import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { FiltersModule } from '@cc/common/filters';
import {
  EnvironmentActionSelectModule,
  EnvironmentApiSelectModule,
  EnvironmentDomainSelectModule,
  UserApiSelectModule,
} from '@cc/common/forms';
import { PagingModule } from '@cc/common/paging';
import { LuApiModule } from '@lucca-front/ng/api';
import { ALuDateAdapter, LuNativeDateAdapter } from '@lucca-front/ng/core';
import { LuDateModule } from '@lucca-front/ng/date';
import { LuInputModule } from '@lucca-front/ng/input';
import {
  LuOptionFeederModule,
  LuOptionModule,
  LuOptionPagerModule,
  LuOptionPickerModule,
  LuOptionSearcherModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { EnvironmentsModule } from '../../domain/environments';
import { AnonymizationButtonGroupComponent } from './components/anonymization-button-group/anonymization-button-group.component';
import { DateRangeSelectComponent } from './components/date-range-select/date-range-select.component';
import { LogsFiltersComponent } from './components/logs-filter/logs-filter.component';
import { LogsListComponent } from './components/logs-list/logs-list.component';
import { LogsComponent } from './logs.component';

const routes: Routes = [
	{ path: '', component: LogsComponent },
];

@NgModule({
	declarations: [
		LogsComponent,
    LogsListComponent,
    LogsFiltersComponent,
    DateRangeSelectComponent,
    AnonymizationButtonGroupComponent,
	],
  imports: [
    RouterModule.forChild(routes),
    ScrollingModule,
    CommonModule,
    LuSelectInputModule,
    FormsModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionSearcherModule,
    LuOptionPagerModule,
    LuOptionModule,
    LuInputModule,
    LuDateModule,
    LuApiModule,
    TranslateModule,
    EnvironmentsModule,
    PagingModule,
    FiltersModule,
    EnvironmentApiSelectModule,
    EnvironmentDomainSelectModule,
    UserApiSelectModule,
    EnvironmentActionSelectModule,
  ],
  providers: [
    LuNativeDateAdapter,
    { provide: ALuDateAdapter, useClass: LuNativeDateAdapter },
  ],
})
export class LogsModule {};
