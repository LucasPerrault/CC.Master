import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {LOCALE_ID, NgModule} from '@angular/core';
import localeFr from '@angular/common/locales/fr';
import {registerLocaleData} from '@angular/common';
import { AppComponent } from './app.component';
import { PrincipalModule } from './aspects/principal';

registerLocaleData(localeFr);

const routes: Routes = [
	{ path: 'logs', loadChildren: () => import('./pages/logs').then(m => m.LogsModule) },
	{ path: '**', redirectTo: 'logs', pathMatch: 'full' },
];

@NgModule({
	declarations: [
		AppComponent,
	],
	imports: [
		BrowserModule,
		RouterModule.forRoot(routes),
		PrincipalModule.forRoot(),
	],
	providers: [
    { provide: LOCALE_ID, useValue: 'fr-FR' },
  ],
	bootstrap: [AppComponent]
})
export class AppModule { }
