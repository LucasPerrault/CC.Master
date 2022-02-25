import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { RangeComponent } from './range.component';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [RangeComponent],
  imports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
  ],
  exports: [RangeComponent],
})
export class RangeModule { }
