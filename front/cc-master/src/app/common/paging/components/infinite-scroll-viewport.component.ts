import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { Component, EventEmitter, HostBinding, Input, Output, ViewChild, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'cc-infinite-scroll-viewport',
  templateUrl: './infinite-scroll-viewport.component.html',
  styleUrls: ['./infinite-scroll-viewport.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class InfiniteScrollViewportComponent {
  @Input() public readonly rowHeightFixed = 42;
  @Input() public readonly rowNumberBeforeBottomToShowMore = 15;
  @Output() public showMore: EventEmitter<void> = new EventEmitter<void>();
  @ViewChild(CdkVirtualScrollViewport, { static: true, read: CdkVirtualScrollViewport })
  public scrollViewport: CdkVirtualScrollViewport;

  @HostBinding('style.--row-height-in-px')
  public readonly rowHeightFixedInPixel = `${this.rowHeightFixed}px`;

  private readonly rowsHeightStepToShowMore = this.rowHeightFixed * this.rowNumberBeforeBottomToShowMore;

  public scroll(): void {
    if (this.scrollViewport.measureScrollOffset('bottom') <= this.rowsHeightStepToShowMore) {
      this.showMore.emit();
    }
  }
}
