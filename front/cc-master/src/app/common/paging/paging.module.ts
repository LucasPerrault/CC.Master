import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { PagingService } from '@cc/common/paging/services/paging.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [PagingService],
})
export class PagingModule { }
