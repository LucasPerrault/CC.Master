import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { PriceGridListComponent } from './price-grid-list.component';

@NgModule({
  declarations: [PriceGridListComponent],
  imports: [
    CommonModule,
    TranslateModule,
  ],
  exports: [PriceGridListComponent],
})
export class PriceGridListModule { }
