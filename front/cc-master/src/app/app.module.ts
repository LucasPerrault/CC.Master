import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { ErrorsModule } from '@cc/aspects/errors';
import { PrincipalModule } from '@cc/aspects/principal';
import { RightsModule } from '@cc/aspects/rights/rights.module';
import { ForbiddenComponent, forbiddenUrl } from '@cc/common/errors';
import { ToastsModule } from '@cc/common/toasts';
import { TranslateModule } from '@cc/aspects/translate';

import { AppComponent } from './app.component';

const routes: Routes = [
  { path: 'logs', loadChildren: () => import('./pages/logs').then(m => m.LogsModule) },
  { path: forbiddenUrl, component: ForbiddenComponent },
  { path: '*', redirectTo: 'logs', pathMatch: 'full' },
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
    ToastsModule,
    RightsModule,
	],
	providers: [
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
}
