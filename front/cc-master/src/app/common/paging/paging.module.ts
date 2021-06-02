import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { PagingService } from '@cc/common/paging/services/paging.service';

import { InfiniteScrollDirective } from './services/infinite-scroll.directive';

@NgModule({
  declarations: [InfiniteScrollDirective],
  imports: [
    CommonModule,
    ScrollingModule,
  ],
  providers: [PagingService],
  exports: [InfiniteScrollDirective],
})
export class PagingModule { }
