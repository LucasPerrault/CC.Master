import { Component, Input } from '@angular/core';
import { IOfferPriceList } from '@cc/domain/billing/offers';

@Component({
  selector: 'cc-price-grid-list',
  templateUrl: './price-grid-list.component.html',
})
export class PriceGridListComponent {
  @Input() public offerPriceList: IOfferPriceList;
}
