import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { HistoryTabComponent } from './history-tab.component';
import { ContractLogsService } from './services/contract-logs.service';

@NgModule({
  declarations: [HistoryTabComponent],
  imports: [
    CommonModule,
    TranslateModule,
  ],
  providers: [
    ContractLogsService,
  ],
})
export class HistoryTabModule { }
