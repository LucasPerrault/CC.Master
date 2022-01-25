import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { isAfter } from 'date-fns';
import { ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take, takeUntil } from 'rxjs/operators';

import { getSpecificAuthor } from '../../constants/specific-author-id.enum';
import { IDemo, IDemoAuthor, ITemplateDemo } from '../../models/demo.interface';
import { ConnectAsDataService } from '../../services/connect-as-data.service';
import { DemosDataService } from '../../services/demos-data.service';
import { DemosListService } from '../../services/demos-list.service';
import { IDemoInstanceUser } from '../selects';

@Component({
  selector: 'cc-demo-card',
  templateUrl: './demo-card.component.html',
  styleUrls: ['./demo-card.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DemoCardComponent implements OnInit, OnDestroy {
  @Input() public demo: IDemo;
  @Input() public templateDemos: ITemplateDemo[];

  @Output() public delete: EventEmitter<IDemo> = new EventEmitter<IDemo>();
  @Output() public showComment: EventEmitter<IDemo> = new EventEmitter<IDemo>();
  @Output() public editComment: EventEmitter<IDemo> = new EventEmitter<IDemo>();
  @Output() public editPassword: EventEmitter<IDemo> = new EventEmitter<IDemo>();

  public formControl = new FormControl();

  public protectionButtonClass$ = new ReplaySubject<string>(1);

  public passwordTooltip$ = new ReplaySubject<string>(1);
  public password$ = new ReplaySubject<string>(1);
  public get hasPassword(): boolean {
    return !this.templateDemos?.map(d => d?.subdomain)?.includes(this.demo?.subdomain);
  }

  private destroy$ = new Subject<void>();

  constructor(
    private translatePipe: TranslatePipe,
    private connectAsService: ConnectAsDataService,
    private dataService: DemosDataService,
    private listService: DemosListService,
  ) { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => this.connectAs(user));

    this.hidePassword();
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getTitle(subdomain: string): string {
    const domain = 'ilucca-demo.net';
    return `${ subdomain }.${ domain }`;
  }

  public getAuthorName(authorId: number, author: IDemoAuthor): string {
    const specificAuthor = getSpecificAuthor(authorId);
    return !!specificAuthor ? `${ specificAuthor.firstName }` : `${ author?.firstName } ${ author.lastName}`;
  }

  public openDemo(href: string): void {
    window.open(href);
  }

  public shouldExpire(demo: IDemo): boolean {
    const defaultDeletionDate = new Date(1, 1, 1);
    return !demo?.instance?.isProtected && isAfter(new Date(demo?.deletionScheduledOn), defaultDeletionDate);
  }

  public hidePassword(): void {
    this.password$.next('*'.repeat(this.demo?.instance?.allUsersImposedPassword?.length));
  }

  public showPassword(): void {
    this.password$.next(this.demo?.instance?.allUsersImposedPassword);
    const translationKey = this.translatePipe.transform('demos_card_password_copy');
    this.passwordTooltip$.next(translationKey);
  }

  public copyPassword(): void {
    void navigator.clipboard.writeText(this.demo?.instance?.allUsersImposedPassword);
    this.passwordTooltip$.next('✅');
  }

  public lock(demo: IDemo): void {
    this.toggleProtection(demo, true);
  }

  public unlock(demo: IDemo): void {
    this.toggleProtection(demo, false);
  }

  public connectAsMasterKey(): void {
    this.connectAsService.getConnectionUrlAsMasterKey$(this.demo?.instanceID)
      .pipe(take(1))
      .subscribe(url => window.open(url));
  }

  private connectAs(user: IDemoInstanceUser): void {
    this.connectAsService.getConnectionUrl$(user?.id, this.demo?.instanceID)
      .pipe(take(1))
      .subscribe(url => window.open(url));
  }

  private toggleProtection(demo: IDemo, isProtected: boolean): void {
    this.dataService.protect$(demo?.instanceID, isProtected)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)),
        finalize(() => this.listService.resetOne(demo?.id)))
      .subscribe(this.protectionButtonClass$);
  }
}
