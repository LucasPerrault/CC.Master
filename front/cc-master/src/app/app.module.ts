import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';
import { ErrorsModule } from '@cc/aspects/errors';
import { PrincipalModule } from '@cc/aspects/principal';
import { RightsModule } from '@cc/aspects/rights/rights.module';
import { TranslateModule } from '@cc/aspects/translate';
import { BannerComponent, BannerModule } from '@cc/common/banner';
import { ForbiddenComponent, forbiddenUrl } from '@cc/common/error-redirections';
import { ToastsModule } from '@cc/common/toasts';

import { AppComponent } from './app.component';

const routes: Routes = [
  { path: forbiddenUrl, component: ForbiddenComponent },
  { path: 'logs', loadChildren: () => import('./pages/logs/logs.module').then(m => m.LogsModule) },
  { path: '**', redirectTo: forbiddenUrl, pathMatch: 'full' },
];

@NgModule({
	declarations: [
		AppComponent,
	],
	imports: [
		BrowserModule,
    BrowserAnimationsModule,
		RouterModule.forRoot(routes),
		PrincipalModule.forRoot(),
    ErrorsModule.forRoot(),
    TranslateModule.forRoot(),
    ToastsModule,
    RightsModule,
    BannerModule,
	],
  bootstrap: [AppComponent, BannerComponent],
})
export class AppModule {
}
