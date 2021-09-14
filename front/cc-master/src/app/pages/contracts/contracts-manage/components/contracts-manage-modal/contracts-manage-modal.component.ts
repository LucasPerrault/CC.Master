import { AfterViewInit, Component, Inject, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { RightsService } from '@cc/aspects/rights';
import { TranslatePipe } from '@cc/aspects/translate';
import { INavigationTab, NavigationPath } from '@cc/common/navigation';
import { IContract } from '@cc/domain/billing/contracts';
import { ILuSidepanelContent, LU_SIDEPANEL_DATA, LuSidepanel } from '@lucca-front/ng/sidepanel';
import { BehaviorSubject, from, Subject } from 'rxjs';
import { catchError, finalize, takeUntil } from 'rxjs/operators';

import { ContractsModalTabPath } from './constants/contracts-modal-tab-path.enum';
import { contractsModalTabs } from './constants/contracts-modal-tabs.const';
import { ContractsManageModalService } from './contracts-manage-modal.service';
import { ContractsManageModalDataService } from './contracts-manage-modal-data.service';

interface IContractsManageModalData {
  contractId: number;
  routerOutletRef: TemplateRef<RouterOutlet>;
}

@Component({
  selector: 'cc-contracts-manage-entry-modal',
  template: '<ng-template><router-outlet></router-outlet></ng-template>',
})
export class ContractsManageEntryModalComponent implements OnDestroy, AfterViewInit {
  @ViewChild(TemplateRef) routerOutletRef: TemplateRef<RouterOutlet>;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private luSidepanel: LuSidepanel,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private contractsManageModalService: ContractsManageModalService,
  ) {}

  public ngAfterViewInit(): void {
    const contractId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'), 10);

    const data = { contractId, routerOutletRef: this.routerOutletRef };
    const dialog = this.luSidepanel.open(ContractsManageModalComponent, data, {
      panelClass: ['lu-popup-panel','mod-sidepanel', 'mod-contracts-sidepanel'],
    });

    dialog.onClose
      .pipe(takeUntil(this.destroy$))
      .subscribe(async () => await this.redirectToParentAsync());

    dialog.onDismiss
      .pipe(takeUntil(this.destroy$))
      .subscribe(async () => await this.redirectToParentAsync());

    this.contractsManageModalService.onClose$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => dialog.close(''));

    this.contractsManageModalService.onDismiss$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => dialog.dismiss());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private async redirectToParentAsync(): Promise<void> {
    await this.router.navigate(['.'], {
      relativeTo: this.activatedRoute.parent,
      queryParamsHandling: 'preserve',
    });
  }
}

@Component({
  selector: 'cc-contracts-manage-modal',
  templateUrl: './contracts-manage-modal.component.html',
  styleUrls: ['./contracts-manage-modal.component.scss'],
})
export class ContractsManageModalComponent implements ILuSidepanelContent, OnInit, OnDestroy {
  public title: string;
  public parentRouterOutletRef: TemplateRef<RouterOutlet>;
  public isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public isNotFound$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  private readonly contractId: number;
  private destroy$: Subject<void> = new Subject<void>();

  public get tabs(): INavigationTab[] {
    return contractsModalTabs.filter(tab =>
      this.rightsService.hasOperationsByRestrictionMode(tab.restriction.operations, tab.restriction.mode),
    );
  }

  constructor(
    private rightsService: RightsService,
    private activatedRoute: ActivatedRoute,
    private manageModalDataService: ContractsManageModalDataService,
    private translatePipe: TranslatePipe,
    private router: Router,
    private contractsManageModalService: ContractsManageModalService,
    @Inject(LU_SIDEPANEL_DATA) data: IContractsManageModalData,
  ) {
    this.contractId = data.contractId;
    this.parentRouterOutletRef = data.routerOutletRef;
  }

  public ngOnInit(): void {
    this.updateContractTitle();

    this.contractsManageModalService.onRefresh$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.updateContractTitle());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getRelativeRouteUrl(url: string): string[] {
    return [NavigationPath.Contracts, NavigationPath.ContractsManage, String(this.contractId), url];
  }

  public scrollLeft(tabsWrapper: Element): void {
    tabsWrapper.scrollTo({ behavior: 'smooth', left: 0 });
  }

  public scrollRight(tabsWrapper: Element): void {
    tabsWrapper.scrollTo({ behavior: 'smooth', left: tabsWrapper.scrollWidth });
  }

  private setTitle(contractName: string): void {
    if (!contractName) {
      return;
    }
    this.title = `${ this.translatePipe.transform('front_contractPage_modalTitle') } ${ contractName }`;
  }

  private updateContractTitle(): void {
    this.isLoading$.next(true);

    this.manageModalDataService.getContractById$(this.contractId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isLoading$.next(false)),
        catchError(() => from(this.redirectToNotFoundPage())),
      )
      .subscribe(contract => this.setTitle(contract?.name));
  }

  private redirectToNotFoundPage(): Promise<IContract> {
    this.isNotFound$.next(true);

    const notFoundUrl = this.getRelativeRouteUrl(ContractsModalTabPath.NotFound);

    return this.router
      .navigate(notFoundUrl, { queryParamsHandling: 'preserve' })
      .then(() => null);
  }
}
