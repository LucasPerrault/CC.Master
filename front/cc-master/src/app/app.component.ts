import { Component, Inject, LOCALE_ID, OnInit } from '@angular/core';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { TranslateInitializationService } from '@cc/aspects/translate';
import { LuSentryLoggerService } from '@lucca/sentry/ng';

@Component({
	selector: 'cc-root',
	templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
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
}
