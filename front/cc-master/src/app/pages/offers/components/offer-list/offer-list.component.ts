import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';
import { ISortParams, SortOrder, SortService } from '@cc/common/sort';
import { LuModal } from '@lucca-front/ng/modal';

import { ContractsRoutingKey } from '../../../contracts/contracts-manage/services/contracts-routing.service';
import { BillingMode, billingModes, IBillingMode } from '../../enums/billing-mode.enum';
import { OfferSortParamKey } from '../../enums/offer-sort-param-key.enum';
import { IDetailedOffer } from '../../models/detailed-offer.interface';
import { getCurrency } from '../../models/offer-currency.interface';
import { OfferRestrictionsService } from '../../services/offer-restrictions.service';
import { OfferArchivingComponent } from '../offer-archiving/offer-archiving.component';
import { IOfferArchivingModalData } from '../offer-archiving/offer-archiving.interface';

@Component({
  selector: 'cc-offer-list',
  templateUrl: './offer-list.component.html',
  styleUrls: ['./offer-list.component.scss'],
})
export class OfferListComponent implements OnInit {
  @Input() public offers: IDetailedOffer[];
  @Input() public sortParams: ISortParams;
  @Output() public sort: EventEmitter<ISortParams> = new EventEmitter<ISortParams>();

  public get hasRightToEdit(): boolean {
    return this.restrictionsService.hasRightToCreateOffers();
  }

  public sortParamKey = OfferSortParamKey;
  public sortOrder = SortOrder;

  constructor(
    private luModal: LuModal,
    private translatePipe: TranslatePipe,
    private sortService: SortService,
    private restrictionsService: OfferRestrictionsService,
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
    return !!billingMode ? this.translatePipe.transform(billingMode.name) : '';
  }

  public getCurrencyName(currencyId: number): string {
    return getCurrency(currencyId)?.name;
  }

  public redirectToContracts(offerId: number): void {
    const contractsUrl = `${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }`;
    const query = `${ ContractsRoutingKey.OfferIds }=${ offerId }`;

    const url = [contractsUrl, query].join('?');
    window.open(url);
  }

  public archive(offer: IDetailedOffer): void {
    const data: IOfferArchivingModalData = { offer, isArchived: true };
    this.luModal.open(OfferArchivingComponent, data);
  }

  public unarchive(offer: IDetailedOffer): void {
    const data: IOfferArchivingModalData = { offer, isArchived: false };
    this.luModal.open(OfferArchivingComponent, data);
  }
}
