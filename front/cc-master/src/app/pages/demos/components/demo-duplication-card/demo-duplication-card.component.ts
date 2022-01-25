import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ToastsService, ToastType } from '@cc/common/toasts';
import { IInstanceDuplication, InstanceDuplicationProgress, InstanceDuplicationsService } from '@cc/domain/instances';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { DemoDuplicationsService } from '../../services/demo-duplications.service';
import { DemosListService } from '../../services/demos-list.service';

@Component({
  selector: 'cc-demo-duplication-card',
  templateUrl: './demo-duplication-card.component.html',
  styleUrls: ['./demo-duplication-card.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DemoDuplicationCardComponent implements OnInit, OnDestroy {
  @Input() public duplicationId: string;

  public duplication$ = new ReplaySubject<IInstanceDuplication>(1);

  public get title$(): Observable<string> {
    return this.duplication$.pipe(map(duplication => this.getTitle(duplication?.targetSubdomain)));
  }

  private destroy$ = new Subject<void>();

  constructor(
    private translatePipe: TranslatePipe,
    private toastsService: ToastsService,
    private listService: DemosListService,
    private demoDuplicationsService: DemoDuplicationsService,
    private instanceDuplicationsService: InstanceDuplicationsService,
  ) {}

  public ngOnInit(): void {
    this.instanceDuplicationsService.listenDuplication$(this.duplicationId)
      .pipe(takeUntil(this.destroy$))
      .subscribe(duplication => this.duplication$.next(duplication));

    this.duplication$
      .pipe(takeUntil(this.destroy$), filter(d => this.isFinished(d)))
      .subscribe(() => this.demoDuplicationsService.remove(this.duplicationId));

    this.duplication$
      .pipe(takeUntil(this.destroy$), filter(d => this.isSuccess(d)))
      .subscribe(() => {
        this.notifySuccess();
        this.demoDuplicationsService.remove(this.duplicationId);
        this.listService.resetAll();
      });

    this.duplication$
      .pipe(takeUntil(this.destroy$), filter(d => this.isFailed(d)))
      .subscribe(() => this.notifyFailure());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getTitle(subdomain: string): string {
    const domain = 'ilucca-demo.net';
    return `${ subdomain }.${ domain }`;
  }

  private isSuccess(duplication: IInstanceDuplication): boolean {
    return duplication?.progress === InstanceDuplicationProgress.FinishedWithSuccess;
  }

  private notifySuccess(): void {
    this.toastsService.addToast({
      type: ToastType.Success,
      message: this.translatePipe.transform('demos_duplication_success'),
    });
  }

  private isFailed(duplication: IInstanceDuplication): boolean {
    return duplication?.progress === InstanceDuplicationProgress.FinishedWithFailure;
  }

  private notifyFailure(): void {
    this.toastsService.addToast({
      type: ToastType.Error,
      message: this.translatePipe.transform('demos_duplication_failure'),
    });
  }

  private isFinished(duplication: IInstanceDuplication): boolean {
    return duplication?.progress === InstanceDuplicationProgress.FinishedWithSuccess
      || duplication?.progress === InstanceDuplicationProgress.FinishedWithFailure;
  }
}
