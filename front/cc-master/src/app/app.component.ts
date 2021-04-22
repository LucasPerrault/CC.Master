import { Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { TitleService } from '@cc/aspects/title';
import { TranslateInitializationService } from '@cc/aspects/translate';
import { NoNavComponent } from '@cc/common/routing';
import { LuSentryLoggerService } from '@lucca/sentry/ng';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
  public isNavigationDisplayed = true;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    @Inject(PRINCIPAL) private principal: IPrincipal,
    @Inject(LOCALE_ID) private localeId: string,
    private translateService: TranslateInitializationService,
    private loggerService: LuSentryLoggerService,
    private titleService: TitleService,
    private router: Router,
  ) {
    this.translateService.initializeTranslations(localeId);
  }

  ngOnInit(): void {
    this.loggerService.init();
    const sentryUser = { ...this.principal, id: this.principal.id.toString(), email: '', personalEmail: '' };
    this.loggerService.setUser(sentryUser);

    this.router.events
      .pipe(takeUntil(this.destroy$), filter(e => e instanceof NavigationEnd))
      .subscribe((activatedRoute: NavigationEnd) => this.titleService.updatePageTitle(activatedRoute.urlAfterRedirects));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public updateNavigationVisibility($event: any): void {
    this.isNavigationDisplayed = !($event as NoNavComponent).isNoNavComponent;
  }
}
