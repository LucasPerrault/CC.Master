import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { EnvironmentGroupsService } from './services/environment-groups.service';
import { EnvironmentsService } from './services/environments.service';
import { LogsService } from './services/logs.service';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [LogsService, EnvironmentsService, EnvironmentGroupsService],
})
export class EnvironmentsModule { }
