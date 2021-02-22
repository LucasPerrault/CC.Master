import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { PagingService } from '@cc/common/paging/services/paging.service';

import { InfiniteScrollViewportComponent } from './components/infinite-scroll-viewport.component';

@NgModule({
  declarations: [InfiniteScrollViewportComponent],
  imports: [
    CommonModule,
    ScrollingModule,
  ],
  providers: [PagingService],
  exports: [
    InfiniteScrollViewportComponent,
  ],
})
export class PagingModule { }
