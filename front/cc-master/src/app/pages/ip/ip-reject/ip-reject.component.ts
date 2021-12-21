import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, SubmissionState, toSubmissionState } from '@cc/common/forms';
import { NoNavComponent } from '@cc/common/routing';
import { ToastsService, ToastType } from '@cc/common/toasts';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, map, take, takeUntil, tap } from 'rxjs/operators';

import { IIpRequestValidity, IpDataService } from '../ip-data.service';
import { RequestValidityState, toRequestValidityState } from '../ip-request-validity-state.enum';

enum IpRejectRoutingParams {
  Ip = 'ip',
  Code = 'code',
}

@Component({
  selector: 'cc-ip-reject',
  templateUrl: './ip-reject.component.html',
  styleUrls: ['./ip-reject.component.scss'],
})
export class IpRejectComponent extends NoNavComponent implements OnInit, OnDestroy {

  public requestValidity$: ReplaySubject<IIpRequestValidity> = new ReplaySubject<IIpRequestValidity>(1);
  public requestValidityState$: ReplaySubject<RequestValidityState> = new ReplaySubject<RequestValidityState>(1);
  public requestValidityState = RequestValidityState;

  public rejectionSubmissionState$: ReplaySubject<SubmissionState> = new ReplaySubject(1);

  public get rejectionButtonClass$(): Observable<string> {
    return this.rejectionSubmissionState$.pipe(map(state => getButtonState(state)));
  }

  public userIp: string;
  public userCode: string;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private activatedRoute: ActivatedRoute,
    private dataService: IpDataService,
    private toastsService: ToastsService,
  ) {
    super();
  }

  public async ngOnInit(): Promise<void> {
    this.requestValidityState$.next(RequestValidityState.Load);

    const params = this.activatedRoute.snapshot.queryParamMap;
    this.userIp = params.get(IpRejectRoutingParams.Ip);
    this.userCode = params.get(IpRejectRoutingParams.Code);

    this.dataService.getValidity$(this.userCode)
      .pipe(take(1), tap(this.requestValidity$.next), toRequestValidityState())
      .subscribe(this.requestValidityState$);

    this.rejectionSubmissionState$
      .pipe(takeUntil(this.destroy$), filter(state => state === SubmissionState.Success))
      .subscribe(() => this.notify());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public reject(): void {
    this.dataService.reject$(this.userCode)
      .pipe(toSubmissionState())
      .subscribe(this.rejectionSubmissionState$);
  }

  private notify(): void {
    this.toastsService.addToast({
        type: ToastType.Success,
        message: 'Un mail a bien été envoyé, merci de nous l\'avoir signalé.',
    });
  }
}
