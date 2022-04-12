import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { ReplaySubject, Subject, timer } from 'rxjs';
import { take, takeUntil, tap } from 'rxjs/operators';

import { IDemo } from '../../../models/demo.interface';
import { ConnectAsDataService } from '../../../services/connect-as-data.service';
import { DemosDataService } from '../../../services/demos-data.service';
import { DemosListService } from '../../../services/demos-list.service';

@Component({
  selector: 'cc-demo-password',
  templateUrl: './demo-password.component.html',
  styleUrls: ['./demo-password.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DemoPasswordComponent implements OnInit, OnDestroy {
  @Input() public set demo(demo: IDemo) { this.demo$.next(demo); };

  public demo$ = new ReplaySubject<IDemo>(1);

  public password = new FormControl();
  public passwordTooltip$ = new ReplaySubject<string>(1);
  public passwordReadonly = true;
  public editPasswordButtonClass$ = new ReplaySubject<string>(1);

  private destroy$ = new Subject<void>();

  constructor(
    private translatePipe: TranslatePipe,
    private connectAsService: ConnectAsDataService,
    private dataService: DemosDataService,
    private listService: DemosListService,
  ) { }

  public ngOnInit(): void {
    this.demo$
      .pipe(takeUntil(this.destroy$))
      .subscribe(demo => this.initPassword(demo));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public editPassword(demo: IDemo): void {
    this.dataService.editPassword$(demo.instanceID, this.password.value)
      .pipe(take(1), tap(() => this.listService.resetOne(demo.id)), toSubmissionState())
      .subscribe(state => this.editPasswordButtonClass$.next(getButtonState(state)));
  }

  public cancelPasswordEdition(demo: IDemo) {
    this.password.setValue(demo?.instance?.allUsersImposedPassword);
    this.togglePasswordReadonly();
  }

  public copyPassword(): void {
    void navigator.clipboard.writeText(this.password.value);
    this.passwordTooltip$.next(this.translatePipe.transform('demos_card_password_copy_validation'));
    timer(1000).subscribe(() => this.resetPasswordTooltip());
  }

  public togglePasswordReadonly(): void {
    this.passwordReadonly = !this.passwordReadonly;
  }

  private initPassword(demo: IDemo): void {
    this.password.setValue(demo?.instance?.allUsersImposedPassword);
    this.resetPasswordTooltip();
  }

  private resetPasswordTooltip(): void {
    this.passwordTooltip$.next(this.translatePipe.transform('demos_card_password_copy'));
  }
}
