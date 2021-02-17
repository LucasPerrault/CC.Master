import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputModule } from '@lucca-front/ng/input';
import { LuOptionModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { EnvironmentActionSelectComponent } from './environment-action-select.component';

@NgModule({
  declarations: [EnvironmentActionSelectComponent],
  imports: [
    FormsModule,
    LuSelectInputModule,
    LuInputModule,
    LuOptionModule,
    TranslateModule,
    CommonModule,
    LuTooltipTriggerModule,
  ],
  exports: [EnvironmentActionSelectComponent],
})
export class EnvironmentActionSelectModule { }
