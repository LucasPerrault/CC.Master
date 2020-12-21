import {Component, Inject, LOCALE_ID, OnInit} from '@angular/core';
import {TranslateInitializationService} from '@cc/aspects/translate';

@Component({
	selector: 'cc-root',
	templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
	constructor(
    @Inject(LOCALE_ID) private localeId: string,
	  private translateService: TranslateInitializationService
  ) {
	  this.translateService.initializeTranslations(localeId);
  }

	ngOnInit(): void {}
}
