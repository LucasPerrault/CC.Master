import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ErrorsModule } from '@cc/aspects/errors';
import { PrincipalModule } from '@cc/aspects/principal';
import { RightsModule } from '@cc/aspects/rights/rights.module';
import { TitleModule } from '@cc/aspects/title';
import { TranslateModule } from '@cc/aspects/translate';
import { BannerComponent, BannerModule } from '@cc/common/banner';
import { NavigationModule } from '@cc/common/navigation/navigation.module';
import { ToastsModule } from '@cc/common/toasts';
import { BillingCoreDataModule } from '@cc/domain/billing/billling-core-data';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

@NgModule({
	declarations: [
		AppComponent,
	],
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    PrincipalModule.forRoot(),
    BillingCoreDataModule.forRoot(),
    ErrorsModule.forRoot(),
    TranslateModule.forRoot(),
    ToastsModule,
    RightsModule,
    BannerModule,
    TitleModule,
    NavigationModule,
  ],
  bootstrap: [AppComponent, BannerComponent],
})
export class AppModule {
}
