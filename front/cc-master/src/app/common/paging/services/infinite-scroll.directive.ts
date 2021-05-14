import { Directive, ElementRef, EventEmitter, HostListener, Output } from '@angular/core';

@Directive({
  selector: '[ccInfiniteScroll]',
})
export class InfiniteScrollDirective {
  @Output() bottomReached: EventEmitter<void> = new EventEmitter<void>();

  constructor(private element: ElementRef) {}

  @HostListener('scroll') public scroll(): void {
    const isBottomReached = this.getScrollPosition() >= this.element.nativeElement.scrollHeight;

    if (isBottomReached) {
      this.bottomReached.emit();
    }
  }

  private getScrollPosition(): number {
    return Math.ceil(this.element.nativeElement.offsetHeight + this.element.nativeElement.scrollTop);
  }
}
