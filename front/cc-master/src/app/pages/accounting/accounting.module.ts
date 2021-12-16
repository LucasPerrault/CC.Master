import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NavigationPath } from '@cc/common/navigation';

import { AccountingComponent } from './accounting.component';
import { AccountingRevenueComponent } from './accounting-revenue/accounting-revenue.component';
import { AccountingRevenueModule } from './accounting-revenue/accounting-revenue.module';
import { MiscellaneousTransactionsComponent } from './miscellaneous-transactions/miscellaneous-transactions.component';
import { MiscellaneousTransactionsModule } from './miscellaneous-transactions/miscellaneous-transactions.module';

const routes: Routes = [
  {
    path: NavigationPath.Accounting,
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: NavigationPath.AccountingRevenue,
      },
      {
        path: NavigationPath.AccountingRevenue,
        component: AccountingRevenueComponent,
      },
      {
        path: NavigationPath.AccountingMiscTransactions,
        component: MiscellaneousTransactionsComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [
    AccountingComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    AccountingRevenueModule,
    MiscellaneousTransactionsModule,
  ],
})
export class AccountingModule { }
