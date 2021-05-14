import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { ApiV3DateService } from './services/api-v3-date.service';

@NgModule({
  declarations: [],
  providers: [
    ApiV3DateService,
  ],
  imports: [
    CommonModule,
  ],
})
export class QueriesModule { }

