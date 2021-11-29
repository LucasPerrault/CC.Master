import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { EstablishmentTypeFilterComponent } from './establishment-type-filter.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [EstablishmentTypeFilterComponent],
  exports: [EstablishmentTypeFilterComponent],
  imports: [
    CommonModule,
    TranslateModule,
    ReactiveFormsModule,
  ],
})
export class EstablishmentTypeFilterModule { }
