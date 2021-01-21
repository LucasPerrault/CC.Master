import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { ErrorsModule } from '@cc/aspects/errors';
import { PrincipalModule } from '@cc/aspects/principal';
import { RightsModule } from '@cc/aspects/rights/rights.module';
import { TranslateModule } from '@cc/aspects/translate';
import { ForbiddenComponent, forbiddenUrl } from '@cc/common/error-redirections';
import { ToastsModule } from '@cc/common/toasts';
import { LuSentryModule } from '@lucca/sentry/ng';

import { environment } from '../environments/environment';
import { AppComponent } from './app.component';

const routes: Routes = [
  { path: forbiddenUrl, component: ForbiddenComponent },
  { path: '*', redirectTo: forbiddenUrl, pathMatch: 'full' },
];

@NgModule({
	declarations: [
		AppComponent,
	],
	imports: [
		BrowserModule,
		RouterModule.forRoot(routes),
		PrincipalModule.forRoot(),
    ErrorsModule.forRoot(),
    TranslateModule.forRoot(),
    LuSentryModule.forRoot({ config: environment.sentry }),
    ToastsModule,
    RightsModule,
	],
	providers: [
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
}
