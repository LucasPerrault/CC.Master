import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { RangeComponent } from './range.component';

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
