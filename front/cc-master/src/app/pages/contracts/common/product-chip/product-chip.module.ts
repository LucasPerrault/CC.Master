import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { ProductChipComponent } from './product-chip.component';

@NgModule({
  declarations: [ProductChipComponent],
  imports: [CommonModule],
  exports: [ProductChipComponent],
})
export class ProductChipModule { }
