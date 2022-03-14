import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NavigationPath } from '@cc/common/navigation';

@Component({
  selector: 'cc-offer-not-found-tab',
  templateUrl: './offer-not-found-tab.component.html',
  styleUrls: ['./offer-not-found-tab.component.scss'],
})
export class OfferNotFoundTabComponent {
  constructor(private router: Router) {
  }

  public redirectToOffers(): void {
    this.router.navigate([NavigationPath.Offers, { queryParamsHandling: 'preserve' }]).then(() => null);
  }
}
