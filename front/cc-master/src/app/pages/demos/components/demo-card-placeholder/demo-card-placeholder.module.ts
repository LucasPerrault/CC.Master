import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { DemoInstanceUserApiSelectModule } from '../selects';
import { DemoCardPlaceholderComponent } from './demo-card-placeholder.component';

@NgModule({
  declarations: [DemoCardPlaceholderComponent],
  exports: [DemoCardPlaceholderComponent],
  imports: [
    CommonModule,
    DemoInstanceUserApiSelectModule,
    ReactiveFormsModule,
    LuTooltipTriggerModule,
    TranslateModule,
  ],
})
export class DemoCardPlaceholderModule {}
