import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { AdvancedFiltersComponent } from './advanced-filters.component';
import { AdvancedFiltersActionsModule } from './components/advanced-filters-actions/advanced-filters-actions.module';
import { ComparisonFilterCriterionFormModule } from './components/comparison-filter-criterion';
import { LogicalOperatorSelectModule } from './components/logical-operator-select';
import { AdvancedFilterApiMappingService } from './services/advanced-filter-api-mapping.service';

@NgModule({
  declarations: [AdvancedFiltersComponent],
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
  exports: [AdvancedFiltersComponent],
  providers: [AdvancedFilterApiMappingService],
})
export class AdvancedFiltersModule { }
