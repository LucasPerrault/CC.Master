import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';
import { ISortParams, SortOrder, SortService } from '@cc/common/sort';

import { ContractsRoutingKey } from '../../../contracts/contracts-manage/services/contracts-routing.service';
import { BillingMode, billingModes, IBillingMode } from '../../enums/billing-mode.enum';
import { OfferSortParamKey } from '../../enums/offer-sort-param-key.enum';
import { IDetailedOffer } from '../../models/detailed-offer.interface';

@Component({
  selector: 'cc-offer-list',
  templateUrl: './offer-list.component.html',
  styleUrls: ['./offer-list.component.scss'],
})
export class OfferListComponent implements OnInit {
  @Input() public offers: IDetailedOffer[];
  @Input() public sortParams: ISortParams;
  @Output() public sort: EventEmitter<ISortParams> = new EventEmitter<ISortParams>();


  public sortParamKey = OfferSortParamKey;
  public sortOrder = SortOrder;

  constructor(
    private translatePipe: TranslatePipe,
    private sortService: SortService,
  ) { }

  ngOnInit(): void {
  }

  public sortBy(field: string, order: SortOrder = SortOrder.Asc): void {
    this.sortParams = this.sortService.updateSortParam(field, order, this.sortParams);
    this.sort.emit(this.sortParams);
  }

  public getSortOrderClass(field: string): string {
    return this.sortService.getSortOrderClass(field, this.sortParams);
  }

  public getBillingMode(mode: BillingMode): string {
    const billingMode: IBillingMode = billingModes.find(m => m.id === mode);
    return this.translatePipe.transform(billingMode.name);
  }

  public redirectToContracts(offerId: number): void {
    const contractsUrl = `${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }`;
    const query = `${ ContractsRoutingKey.OfferIds }=${ offerId }`;

    const url = [contractsUrl, query].join('?');
    window.open(url);
  }

  public canBeDeleted(offer: IDetailedOffer): boolean {
    return offer.activeContractNumber === 0;
  }
}
