import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { AdvancedFilterFormComponent } from './advanced-filter-form.component';
import { AdvancedFilterFormService } from './advanced-filter-form.service';
import { ComparisonFilterCriterionFormModule } from './components/comparison-filter-criterion';
import { LogicalOperatorSelectModule } from './components/logical-operator-select';

@NgModule({
  declarations: [AdvancedFilterFormComponent],
  imports: [
    CommonModule,
    LogicalOperatorSelectModule,
    ReactiveFormsModule,
    LuTooltipTriggerModule,
    ComparisonFilterCriterionFormModule,
    TranslateModule,
    LogicalOperatorSelectModule,
  ],
  exports: [AdvancedFilterFormComponent],
  providers: [AdvancedFilterFormService]
})
export class AdvancedFilterFormModule { }
