import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FiltersService } from '@cc/common/filters/services/filters.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [FiltersService],
})
export class FiltersModule { }
