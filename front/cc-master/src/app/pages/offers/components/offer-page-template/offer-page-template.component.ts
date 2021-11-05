import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { NavigationPath } from '@cc/common/navigation';

@Component({
  selector: 'cc-offer-template',
  templateUrl: './offer-page-template.component.html',
  styleUrls: ['./offer-page-template.component.scss'],
})
export class OfferPageTemplateComponent {
  @Input() public title: string;

  constructor(private router: Router) { }

  public redirectToOffers(): void {
    this.router.navigate([NavigationPath.Offers]);
  }
}
