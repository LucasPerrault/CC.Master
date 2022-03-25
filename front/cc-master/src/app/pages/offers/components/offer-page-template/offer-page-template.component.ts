import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { INavigationTab, NavigationPath } from '@cc/common/navigation';

@Component({
  selector: 'cc-offer-template',
  templateUrl: './offer-page-template.component.html',
  styleUrls: ['./offer-page-template.component.scss'],
})
export class OfferPageTemplateComponent {
  @Input() public title: string;
  @Input() public tabs: INavigationTab[] = [];

  constructor(private router: Router) { }

  public redirectToOffers(): void {
    this.router.navigate([NavigationPath.Offers], { queryParamsHandling: 'preserve' });
  }
}
