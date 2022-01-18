import { Component, EventEmitter, Input, Output } from '@angular/core';
import { getButtonState, SubmissionState } from '@cc/common/forms';
import { NavigationPath } from '@cc/common/navigation';
import { IContract } from '@cc/domain/billing/contracts';

@Component({
  selector: 'cc-counts-dashboard-contracts-table',
  templateUrl: './counts-dashboard-contracts-table.component.html',
  styleUrls: ['./counts-dashboard-contracts-table.component.scss'],
})
export class CountsDashboardContractsTableComponent {
  @Input() public contracts: IContract[];
  @Input() public totalCounts: number;
  @Input() public isLoading: boolean;
  @Input() public set redirectionState(state: SubmissionState) {
    this.redirectionButtonClass = getButtonState(state);
  }
  @Output() public showAllContracts: EventEmitter<void> = new EventEmitter<void>();

  public redirectionButtonClass: string;

  public get isDisabled(): boolean {
    const maxQueryString = 200;
    return this.totalCounts > maxQueryString;
  }

  constructor() { }

  public showContracts(): void {
    this.showAllContracts.emit();
  }

  public redirectToContract(contractId: number): void {
    window.open(`${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }/${ contractId }`);
  }

  public trackBy(index: number, contract: IContract): number {
    return contract.id;
  }

}
