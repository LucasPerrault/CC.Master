import { Component, Input } from '@angular/core';
import { IOfferPriceList, IPriceRow } from '@cc/domain/billing/offers';

@Component({
  selector: 'cc-price-grid-list',
  templateUrl: './price-grid-list.component.html',
})
export class PriceGridListComponent {
  @Input() public offerPriceList: IOfferPriceList;

  public get priceRows(): IPriceRow[] {
    const priceList = this.offerPriceList.priceLists[0];
    return priceList.rows;
  }

  public getLowerBound(maxIncludedCount: number): number {
    const currentRowIndex = this.priceRows.findIndex(row => row.maxIncludedCount === maxIncludedCount);
    const previousMaxIncludedCount = this.priceRows[currentRowIndex - 1]?.maxIncludedCount;

    return !!previousMaxIncludedCount ? previousMaxIncludedCount + 1 : 0;
  }
}
