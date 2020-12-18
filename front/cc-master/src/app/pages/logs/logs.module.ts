import { NgModule } from '@angular/core';
import {CommonModule} from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import {ScrollingModule} from '@angular/cdk/scrolling';
import {LogsService} from './services';
import { LogsComponent } from './logs.component';
import {LogsListComponent} from './components/logs-list/logs-list.component';

const routes: Routes = [
	{ path: '', component: LogsComponent }
];

@NgModule({
	declarations: [
		LogsComponent,
    LogsListComponent
	],
  imports: [
    RouterModule.forChild(routes),
    ScrollingModule,
    CommonModule,
  ],
  providers: [LogsService]
})
export class LogsModule {};
