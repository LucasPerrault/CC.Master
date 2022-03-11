import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';
import { ReplaySubject } from 'rxjs';

import { ContractState } from '../../../../../contracts/contracts-manage/constants/contract-state.enum';
import { ContractsRoutingKey } from '../../../../../contracts/contracts-manage/services/contracts-routing.service';

@Component({
  selector: 'cc-counts-dashboard-card',
  templateUrl: './counts-dashboard-card.component.html',
  styleUrls: ['./counts-dashboard-card.component.scss'],
})
export class CountsDashboardCardComponent {
  @Input() public title: string;
  @Input() public totalCount: number;
  @Input() public contractIds: number[];

  public copyTooltip$ = new ReplaySubject<string>(1);

  public get hasContracts(): boolean {
    return !!this.contractIds?.length;
  }

  public get hasTooManyContracts(): boolean {
    const maxQueryString = 100;
    return this.contractIds?.length > maxQueryString;
  }

  constructor(private translatePipe: TranslatePipe) {}

  public copyToClipboard(): void {
    navigator.clipboard.writeText(this.contractIds.join(','));
    this.copyTooltip$.next('✅');
  }

  public redirect(): void {
    const url = `${ NavigationPath.Contracts}/${ NavigationPath.ContractsManage }`;
    const query = [
      `${ContractsRoutingKey.Ids}=${this.contractIds.join(',')}`,
      `${ContractsRoutingKey.State}=${ ContractState.NotStarted },${ ContractState.InProgress},${ ContractState.Closed }`,
    ];

    const redirectionUrl = !!this.contractIds?.length ? `${url}?${ query.join('&') }` : url;
    window.open(redirectionUrl);
  }

  public resetCopyTooltip(): void {
    this.copyTooltip$.next(this.translatePipe.transform('counts_copy_contract_ids_to_clipboard'));
  }
}
