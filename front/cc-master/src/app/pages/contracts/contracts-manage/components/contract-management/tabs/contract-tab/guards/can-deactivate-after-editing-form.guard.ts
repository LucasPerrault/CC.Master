import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';

import { ContractTabComponent } from '../contract-tab.component';

@Injectable()
export class CanDeactivateAfterEditingForm implements CanDeactivate<ContractTabComponent> {
  canDeactivate(
    component: ContractTabComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState: RouterStateSnapshot,
  ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
    if (!component.hasFormChanged() || component.isClosePopupConfirmed$.value) {
      return of(true);
    }

    const popupRef = component.openCloseConfirmationPopup();
    return popupRef.onClose;
  }
}
