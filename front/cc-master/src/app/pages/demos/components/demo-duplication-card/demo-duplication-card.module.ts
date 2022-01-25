import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { DemoInstanceUserApiSelectModule } from '../selects';
import { DemoDuplicationCardComponent } from './demo-duplication-card.component';

@NgModule({
  declarations: [DemoDuplicationCardComponent],
  exports: [DemoDuplicationCardComponent],
  imports: [
    CommonModule,
    DemoInstanceUserApiSelectModule,
    ReactiveFormsModule,
    LuTooltipTriggerModule,
    TranslateModule,
  ],
})
export class DemoDuplicationCardModule {}
