import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { DemoInstanceUserApiSelectModule } from '../selects';
import { DemoCardComponent } from './demo-card.component';

@NgModule({
  declarations: [DemoCardComponent],
  exports: [DemoCardComponent],
  imports: [
    CommonModule,
    DemoInstanceUserApiSelectModule,
    ReactiveFormsModule,
    LuTooltipTriggerModule,
    TranslateModule,
  ],
})
export class DemoCardModule {}
