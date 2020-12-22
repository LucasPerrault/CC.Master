import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { PrincipalModule } from '@cc/aspects/principal';
import { TranslateModule } from '@cc/aspects/translate';

import { AppComponent } from './app.component';

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
    TranslateModule.forRoot(),
	],
	providers: [
  ],
	bootstrap: [AppComponent],
})
export class AppModule { }
