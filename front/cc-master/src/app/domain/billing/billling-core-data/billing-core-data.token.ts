import { InjectionToken } from '@angular/core';
import { IBillingCoreData } from '@cc/domain/billing/billling-core-data/billing-core-data.interface';

export const BILLING_CORE_DATA = new InjectionToken<IBillingCoreData>('BillingCoreData');
