import { Directionality } from '@angular/cdk/bidi';
import {
  Overlay,
  OverlayContainer,
  OverlayKeyboardDispatcher,
  OverlayOutsideClickDispatcher,
  OverlayPositionBuilder,
  ScrollStrategyOptions,
} from '@angular/cdk/overlay';
import { DOCUMENT, Location } from '@angular/common';
import { ComponentFactoryResolver, Inject,Injectable, Injector, NgZone } from '@angular/core';

@Injectable()
export class LuccaOverlayContainer extends OverlayContainer {
  // eslint-disable-next-line @typescript-eslint/naming-convention
  protected override _createContainer(): void {
    super._createContainer();

    const container = this.getContainerElement();
    container.style.zIndex = '10000';
  }
}

@Injectable()
export class LuccaOverlay extends Overlay {
  public constructor(
    scrollStrategies: ScrollStrategyOptions,
    overlayContainer: LuccaOverlayContainer, // Only OverlayContainer is overriden
    componentFactoryResolver: ComponentFactoryResolver,
    positionBuilder: OverlayPositionBuilder,
    keyboardDispatcher: OverlayKeyboardDispatcher,
    injector: Injector,
    ngZone: NgZone,
    @Inject(DOCUMENT) document: Document,
    directionality: Directionality,
    location: Location,
    outsideClickDispatcher: OverlayOutsideClickDispatcher,
  ) {
    super(
      scrollStrategies,
      overlayContainer,
      componentFactoryResolver,
      positionBuilder,
      keyboardDispatcher,
      injector,
      ngZone,
      document,
      directionality,
      location,
      outsideClickDispatcher,
    );
  }
}
