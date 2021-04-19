import { Component, Inject, LOCALE_ID, OnInit } from '@angular/core';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { TranslateInitializationService } from '@cc/aspects/translate';
import { NoNavComponent } from '@cc/common/error-redirections';
import { LuSentryLoggerService } from '@lucca/sentry/ng';

@Component({
	selector: 'cc-root',
	templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public isNavigationDisplayed = true;

	constructor(
    @Inject(PRINCIPAL) private principal: IPrincipal,
    @Inject(LOCALE_ID) private localeId: string,
	  private translateService: TranslateInitializationService,
    private loggerService: LuSentryLoggerService,
  ) {
	  this.translateService.initializeTranslations(localeId);
  }

	ngOnInit(): void {
	  this.loggerService.init();
    const sentryUser = { ...this.principal, id: this.principal.id.toString(), email: '', personalEmail: '' };
    this.loggerService.setUser(sentryUser);
  }

  public updateNavigationVisibility($event: any): void {
    this.isNavigationDisplayed = !($event as NoNavComponent).isNoNavComponent;
  }
}
