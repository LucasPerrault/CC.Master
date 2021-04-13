import { DOCUMENT } from '@angular/common';
import { Directive, EventEmitter, HostListener, Inject, Output } from '@angular/core';

@Directive({
  selector: '[ccInfiniteScroll]',
})
export class InfiniteScrollDirective {
  @Output() bottomReached: EventEmitter<void> = new EventEmitter<void>();

  constructor(@Inject(DOCUMENT) private document: Document) {}

  @HostListener('document:scroll') public scroll(): void {
    const isBottomReached = this.getScrollPosition() >= this.document.documentElement.scrollHeight;

    if (isBottomReached) {
      this.bottomReached.emit();
    }
  }

  private getScrollPosition(): number {
    return Math.ceil(this.document.documentElement.offsetHeight + this.document.scrollingElement.scrollTop);
  }
}
