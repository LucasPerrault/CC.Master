import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import {
  DateRangeSelectModule,
  EnvironmentActionSelectModule,
  EnvironmentApiSelectModule,
  EnvironmentDomainSelectModule,
  UserApiSelectModule,
} from '@cc/common/forms';
import { PagingModule } from '@cc/common/paging';
import { SortModule } from '@cc/common/sort';
import { EnvironmentsModule } from '@cc/domain/environments';
import { UsersModule } from '@cc/domain/users';

import { AnonymizationButtonGroupComponent } from './components/anonymization-button-group/anonymization-button-group.component';
import { LogsFiltersComponent } from './components/logs-filter/logs-filter.component';
import { LogsListComponent } from './components/logs-list/logs-list.component';
import { LogsComponent } from './logs.component';
import { LogsApiMappingService } from './services/logs-api-mapping.service';

const routes: Routes = [
	{ path: '', component: LogsComponent },
];

@NgModule({
	declarations: [
		LogsComponent,
    LogsListComponent,
    LogsFiltersComponent,
    AnonymizationButtonGroupComponent,
	],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    FormsModule,
    EnvironmentsModule,
    EnvironmentApiSelectModule,
    EnvironmentDomainSelectModule,
    EnvironmentActionSelectModule,
    UserApiSelectModule,
    DateRangeSelectModule,
    ScrollingModule,
    PagingModule,
    TranslateModule,
    SortModule,
    UsersModule,
  ],
  providers:[LogsApiMappingService],
})
export class LogsModule {}
