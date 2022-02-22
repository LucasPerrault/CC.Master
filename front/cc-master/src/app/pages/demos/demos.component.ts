import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ApiStandard } from '@cc/common/queries';
import { IUser } from '@cc/domain/users/v4';
import { LuModal } from '@lucca-front/ng/modal';
import { BehaviorSubject, Observable, ReplaySubject, Subject } from 'rxjs';
import { map, skip, take, takeUntil } from 'rxjs/operators';

import { DemoCommentModalComponent } from './components/modals/demo-comment-modal/demo-comment-modal.component';
import { DemoCommentModalMode } from './components/modals/demo-comment-modal/demo-comment-modal-data.interface';
import { DemoDeletionModalComponent } from './components/modals/demo-deletion-modal/demo-deletion-modal.component';
import { DemoPasswordEditionModalComponent } from './components/modals/demo-password-edition-modal/demo-password-edition-modal.component';
import { IDemo, ITemplateDemo } from './models/demo.interface';
import { DemoFilterFormKey } from './models/demo-filters.interface';
import { DemoDuplicationsService } from './services/demo-duplications.service';
import { DemosApiMappingService } from './services/demos-api-mapping.service';
import { DemosDataService } from './services/demos-data.service';
import { DemosListService } from './services/demos-list.service';

@Component({
  selector: 'cc-demos',
  templateUrl: './demos.component.html',
  styleUrls: ['./demos.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DemosComponent implements OnInit, OnDestroy {


  public get duplicationIds$(): Observable<string[]> {
    return this.duplicationsService.duplicationIds$;
  }

  public get hasDemos$(): Observable<boolean> { return this.demos$.pipe(map(demos => !!demos?.length)); }
  public get demos$(): Observable<IDemo[]> { return this.demos.asObservable(); }
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public isLoadingMore$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public templateDemos$ = new ReplaySubject<ITemplateDemo[]>(1);
  public filters = new FormControl();

  private demos = new BehaviorSubject<IDemo[]>([]);
  private paginatedDemos: PaginatedList<IDemo>;
  private destroy$ = new Subject<void>();

  constructor(
    @Inject(PRINCIPAL) private principal: IPrincipal,
    private pagingService: PagingService,
    private dataService: DemosDataService,
    private apiMappingService: DemosApiMappingService,
    private listService: DemosListService,
    private duplicationsService: DemoDuplicationsService,
    private luModal: LuModal,
  ) { }

  public ngOnInit(): void {
    this.dataService.getTemplateDemos$()
      .pipe(take(1))
      .subscribe(this.templateDemos$);

    this.filters.valueChanges
      .pipe(takeUntil(this.destroy$), map(f => this.apiMappingService.toHttpParams(f)))
      .subscribe(httpParams => this.paginatedDemos.updateHttpParams(httpParams));

    this.listService.resetAll$
      .pipe(takeUntil(this.destroy$), map(() => this.apiMappingService.toHttpParams(this.filters.value)))
      .subscribe(httpParams => this.paginatedDemos.updateHttpParams(httpParams));

    this.listService.resetOne$
      .pipe(takeUntil(this.destroy$), map(updatedDemo => this.updateDemos(updatedDemo, this.demos.value)))
      .subscribe(demos => this.demos.next(demos));

    this.paginatedDemos = this.pagingService.paginate<IDemo>(
      (httpParams) => this.getDemos$(httpParams),
      { page: defaultPagingParams.page, limit: 20 },
      ApiStandard.V4,
    );

    this.paginatedDemos.items$
      .pipe(takeUntil(this.destroy$))
      .subscribe(this.demos);

    this.paginatedDemos.state$
      .pipe(skip(1), map(state => state === PaginatedListState.Update))
      .subscribe(this.isLoading$);

    this.paginatedDemos.state$
      .pipe(map(state => state === PaginatedListState.LoadMore))
      .subscribe(this.isLoadingMore$);

    this.initDefaultFilter();
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedDemos.nextPage();
  }

  public openDeletionModal(demo: IDemo): void {
    this.luModal.open(DemoDeletionModalComponent, demo);
  }

  public openCommentModal(demo: IDemo): void {
    const mode = DemoCommentModalMode.Readonly;
    this.luModal.open(DemoCommentModalComponent, { demo, mode });
  }

  public openCommentEditionModal(demo: IDemo): void {
    const mode = DemoCommentModalMode.Edition;
    this.luModal.open(DemoCommentModalComponent, { demo, mode });
  }

  public openPasswordEditionModal(demo: IDemo): void {
    this.luModal.open(DemoPasswordEditionModalComponent, demo);
  }

  private getDemos$(httpParams: HttpParams): Observable<IPaginatedResult<IDemo>> {
    return this.dataService.getDemos$(httpParams)
      .pipe(map(data => ({ items: data?.items, totalCount: data.count })));
  }

  private initDefaultFilter(): void {
    const principalAsUser = { id: this.principal.id, firstName: this.principal.name, lastName: '' } as IUser;
    this.filters.setValue({
      [DemoFilterFormKey.Author]: principalAsUser,
      [DemoFilterFormKey.IsProtected]: false,
    });
  }

  private updateDemos(updatedDemo: IDemo, allDemos: IDemo[]): IDemo[] {
    const updatedDemoIndex = allDemos.findIndex(d => d?.id === updatedDemo?.id);
    allDemos.splice(updatedDemoIndex, 1, updatedDemo);
    return allDemos;
  }

}
