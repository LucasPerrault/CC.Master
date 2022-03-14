import { OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { ComponentType } from '@angular/cdk/portal/portal';
import { Injectable } from '@angular/core';
import { ILuccaModal } from '@cc/common/calendly/services/lucca-modal/lucca-modal.interface';
import { LuccaOverlay } from '@cc/common/calendly/services/lucca-modal/lucca-modal.overlay';
import { Observable } from 'rxjs';
import { finalize, mapTo, take } from 'rxjs/operators';

@Injectable()
export class LuccaModalHandler {
  private overlayRef?: OverlayRef;

  public constructor(private overlay: LuccaOverlay) { }

  public openModal<T extends ILuccaModal>(component: ComponentType<T>, panelClass: string[] = []): Observable<void> {
    this.overlayRef = this.overlay.create({
      hasBackdrop: true,
      scrollStrategy: this.overlay.scrollStrategies.block(),
      positionStrategy: this.overlay.position().global().centerHorizontally().centerVertically(),
      backdropClass: ['cdk-overlay-dark-backdrop', 'lu-popup-backdrop'],
      panelClass: ['lu-popup-panel', ...panelClass],
    });

    this.addLuccaIdentityTheme(this.overlayRef);

    const portal = new ComponentPortal(component);
    const componentRef = this.overlayRef.attach(portal);

    componentRef.changeDetectorRef.detectChanges();

    return componentRef.instance.close$
      .pipe(
        take(1),
        finalize(() => this.closeModal()),
        mapTo(undefined as void),
      );
  }

  public closeModal(): void {
    this.overlayRef?.detach();
  }

  private addLuccaIdentityTheme(overlayRef: OverlayRef): void {
    const overlayStyle = overlayRef.overlayElement.style;
    overlayStyle.setProperty('--palettes-primary-color', '#FFB900');
    overlayStyle.setProperty('--palettes-primary-text', '#121212');
    overlayStyle.setProperty('--palettes-primary-50', '#FFF8E5');
    overlayStyle.setProperty('--palettes-primary-100', '#FFF4D6');
    overlayStyle.setProperty('--palettes-primary-200', '#FDEFC9');
    overlayStyle.setProperty('--palettes-primary-300', '#FDE9B4');
    overlayStyle.setProperty('--palettes-primary-400', '#FBDA83');
    overlayStyle.setProperty('--palettes-primary-500', '#FAD36B');
    overlayStyle.setProperty('--palettes-primary-600', '#FFCA3D');
    overlayStyle.setProperty('--palettes-primary-700', '#FFB900');
    overlayStyle.setProperty('--palettes-primary-800', '#FAA200');
    overlayStyle.setProperty('--palettes-primary-900', '#EB7900');
  }
}
