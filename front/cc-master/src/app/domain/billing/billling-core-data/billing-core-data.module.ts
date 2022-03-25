import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, ModuleWithProviders, NgModule } from '@angular/core';
import {
  BillingCoreDataInitializer, getBillingCoreData,
  initBillingCoreData,
} from '@cc/domain/billing/billling-core-data/billing-core-data.initializer';
import { BILLING_CORE_DATA } from '@cc/domain/billing/billling-core-data/billing-core-data.token';

@NgModule({
  imports: [
    HttpClientModule,
  ],
})
export class BillingCoreDataModule {

  public static forRoot(): ModuleWithProviders<BillingCoreDataModule> {
    return {
      ngModule: BillingCoreDataModule,
      providers: [
        BillingCoreDataInitializer,
        {
          provide: APP_INITIALIZER,
          useFactory: initBillingCoreData,
          deps: [BillingCoreDataInitializer],
          multi: true,
        },
        {
          provide: BILLING_CORE_DATA,
          useFactory: getBillingCoreData,
          deps: [BillingCoreDataInitializer],
          multi: false,
        },
      ],
    };
  }
}
