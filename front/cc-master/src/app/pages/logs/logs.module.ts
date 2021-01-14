import { NgModule } from '@angular/core';
import {CommonModule} from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import {ScrollingModule} from '@angular/cdk/scrolling';
import {LogsService} from './services';
import { LogsComponent } from './logs.component';
import {LogsListComponent} from './components/logs-list/logs-list.component';
import { EnvironmentDomainSelectComponent } from './components/environment-domain-select/environment-domain-select.component';
import {LuSelectInputModule} from '@lucca-front/ng/select';
import {FormsModule} from '@angular/forms';
import {
  LuOptionFeederModule,
  LuOptionModule,
  LuOptionPagerModule,
  LuOptionPickerModule,
  LuOptionSearcherModule
} from '@lucca-front/ng/option';
import {LuInputModule} from '@lucca-front/ng/input';
import {LogsFiltersComponent} from './components/logs-filter/logs-filter.component';
import { EnvironmentActionSelectComponent } from './components/environment-action-select/environment-action-select.component';
import { DateRangeSelectComponent } from './components/date-range-select/date-range-select.component';
import {LuDateModule} from '@lucca-front/ng/date';
import {ALuDateAdapter, LuNativeDateAdapter} from '@lucca-front/ng/core';
import { EnvironmentApiSelectComponent } from './components/environment-api-select/environment-api-select.component';
import {LuApiModule} from '@lucca-front/ng/api';
import { UserApiSelectComponent } from './components/user-api-select/user-api-select.component';
import {TranslateModule} from '@cc/aspects/translate';

const routes: Routes = [
	{ path: '', component: LogsComponent }
];

@NgModule({
	declarations: [
		LogsComponent,
    LogsListComponent,
    LogsFiltersComponent,
    EnvironmentDomainSelectComponent,
    EnvironmentActionSelectComponent,
    DateRangeSelectComponent,
    EnvironmentApiSelectComponent,
    UserApiSelectComponent,
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
  ],
  providers: [
    LogsService,
    LuNativeDateAdapter,
    {provide: ALuDateAdapter, useClass: LuNativeDateAdapter}
  ]
})
export class LogsModule {};
