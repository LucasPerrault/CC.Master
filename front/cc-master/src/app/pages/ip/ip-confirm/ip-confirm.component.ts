import {Component, OnInit} from '@angular/core';
import {FormControl} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {getButtonState, SubmissionState, toSubmissionState} from '@cc/common/forms';
import {NoNavComponent} from '@cc/common/routing';
import {ReplaySubject} from 'rxjs';
import {debounceTime, filter, map, take, tap} from 'rxjs/operators';

import {IIpRequestValidity, IpDataService} from '../ip-data.service';
import {RequestValidityState, toRequestValidityState} from '../ip-request-validity-state.enum';
import {
  AuthorizationDuration,
  authorizationDurations,
  getAuthorization,
  IAuthorizationDuration,
} from './authorization-duration.interface';

enum IpConfirmRoutingParams {
  Code = 'code',
  Redirection = 'redirection',
}

@Component({
  selector: 'cc-ip-confirm',
  templateUrl: './ip-confirm.component.html',
  styleUrls: ['./ip-confirm.component.scss'],
})
export class IpConfirmComponent extends NoNavComponent implements OnInit {

  public authorizationDurations = authorizationDurations;
  public authorizationDuration: FormControl = new FormControl(getAuthorization(AuthorizationDuration.OneDay));
  public authorizationButtonClass$: ReplaySubject<string> = new ReplaySubject(1);

  public requestValidity$: ReplaySubject<IIpRequestValidity> = new ReplaySubject<IIpRequestValidity>(1);
  public redirection$: ReplaySubject<SubmissionState> = new ReplaySubject<SubmissionState>(1);
  public requestValidityState$: ReplaySubject<RequestValidityState> = new ReplaySubject<RequestValidityState>(1);
  public requestValidityState = RequestValidityState;

  public redirection: string;
  public userCode: string;

  private get fullRedirectionHref(): string {
    return `${window.location.protocol}//${window.location.host}${this.redirection}`;
  }

  constructor(private activatedRoute: ActivatedRoute, private dataService: IpDataService) {
    super();
  }

  public async ngOnInit(): Promise<void> {
    this.requestValidityState$.next(RequestValidityState.Load);

    const params = this.activatedRoute.snapshot.queryParamMap;
    this.userCode = params.get(IpConfirmRoutingParams.Code);
    this.redirection = params.get(IpConfirmRoutingParams.Redirection);

    if (!this.userCode) {
      this.requestValidityState$.next(RequestValidityState.HasNoUserCode);
      return;
    }

    this.dataService.getValidity$(this.userCode)
      .pipe(take(1), tap(this.requestValidity$), toRequestValidityState())
      .subscribe(this.requestValidityState$);

    this.redirection$.pipe(
      filter(state => state === SubmissionState.Success ),
      debounceTime(1000),
      ).subscribe(_ => window.location.href = this.fullRedirectionHref)
  }

  public authorize(code: string, duration: IAuthorizationDuration): void {
    this.dataService.confirm$(code, duration?.id)
      .pipe(toSubmissionState(), tap(this.redirection$), map(state => getButtonState(state)))
      .subscribe(this.authorizationButtonClass$)
  }
}
