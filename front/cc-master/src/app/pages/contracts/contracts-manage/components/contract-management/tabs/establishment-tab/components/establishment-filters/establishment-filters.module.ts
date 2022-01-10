import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { EstablishmentTypeFilterModule } from './establiment-type-filter/establishment-type-filter.module';
import { EstablishmentFiltersComponent } from './establishment-filters.component';


@NgModule({
  declarations: [
    EstablishmentFiltersComponent,
  ],
  exports: [
    EstablishmentFiltersComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    EstablishmentTypeFilterModule,
    LuTooltipTriggerModule,
    TranslateModule,
  ],
})
export class EstablishmentFiltersModule { }
