import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { AdvancedFilterFormComponent } from './advanced-filter-form.component';
import { AdvancedFiltersActionsModule } from './components/advanced-filters-actions/advanced-filters-actions.module';
import { ComparisonFilterCriterionFormModule } from './components/comparison-filter-criterion';
import { LogicalOperatorSelectModule } from './components/logical-operator-select';
import { AdvancedFilterApiMappingService } from './services/advanced-filter-api-mapping.service';

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
    AdvancedFiltersActionsModule,
  ],
  exports: [AdvancedFilterFormComponent],
  providers: [AdvancedFilterApiMappingService],
})
export class AdvancedFilterFormModule { }
