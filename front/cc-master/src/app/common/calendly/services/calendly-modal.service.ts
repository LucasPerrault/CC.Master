import { Injectable, OnDestroy } from '@angular/core';
import { CalendlyModalComponent } from '@cc/common/calendly/components';
import { LuccaModalHandler } from '@cc/common/calendly/services/lucca-modal';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Injectable()
export class CalendlyModalService implements OnDestroy {

  private destroy$ = new Subject<void>();

  constructor(private handler: LuccaModalHandler) {}

  public openCalendlyModal() {
    this.handler.openModal(CalendlyModalComponent, ['calendly-popup-panel'])
      .pipe(takeUntil(this.destroy$))
      .subscribe();
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
