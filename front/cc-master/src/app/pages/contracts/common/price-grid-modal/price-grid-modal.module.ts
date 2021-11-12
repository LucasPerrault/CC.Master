import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { PriceGridListModule } from '../price-grid-list/price-grid-list.module';
import { PriceGridModalComponent } from './price-grid-modal.component';
import { PriceGridModalDataService } from './price-grid-modal-data.service';

@NgModule({
  declarations: [PriceGridModalComponent],
  imports: [
    CommonModule,
    TranslateModule,
    PriceGridListModule,
  ],
  exports: [PriceGridModalComponent],
  providers: [PriceGridModalDataService],
})
export class PriceGridModalModule { }
