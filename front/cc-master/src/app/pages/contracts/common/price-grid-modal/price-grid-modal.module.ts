import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { PriceListService } from '../../services/price-list.service';
import { PriceGridListModule } from '../price-grid-list/price-grid-list.module';
import { PriceGridModalComponent } from './price-grid-modal.component';

@NgModule({
  declarations: [PriceGridModalComponent],
  imports: [
    CommonModule,
    TranslateModule,
    PriceGridListModule,
  ],
  exports: [PriceGridModalComponent],
  providers: [PriceListService],
})
export class PriceGridModalModule { }
