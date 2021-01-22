import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { LogsService } from './services/logs.service';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [LogsService],
})
export class EnvironmentsModule { }
