import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputModule } from '@lucca-front/ng/input';
import { LuOptionModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { EnvironmentDomainSelectComponent } from './environment-domain-select.component';

@NgModule({
  declarations: [EnvironmentDomainSelectComponent],
  imports: [
    FormsModule,
    LuSelectInputModule,
    LuInputModule,
    LuOptionModule,
    TranslateModule,
    LuTooltipTriggerModule,
    CommonModule,
  ],
  exports: [EnvironmentDomainSelectComponent],
})
export class EnvironmentDomainSelectModule { }
