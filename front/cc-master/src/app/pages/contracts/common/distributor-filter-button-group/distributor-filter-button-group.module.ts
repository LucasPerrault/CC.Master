import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { DistributorFilterButtonGroupComponent } from './distributor-filter-button-group.component';

@NgModule({
  declarations: [DistributorFilterButtonGroupComponent],
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
  ],
  exports: [DistributorFilterButtonGroupComponent],
})
export class DistributorFilterButtonGroupModule { }
