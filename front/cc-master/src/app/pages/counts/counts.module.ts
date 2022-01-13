import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Operation, OperationsGuard } from '@cc/aspects/rights';
import { TranslateModule } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';

import { CountsNavigationPath } from './common/enums/counts-navigation-path.enum';
import { CountsComponent } from './counts.component';
import { CountsLauncherComponent } from './counts-launcher/counts-launcher.component';
import { CountsLauncherModule } from './counts-launcher/counts-launcher.module';
import { CountsTablePageComponent } from './counts-table-page/counts-table-page.component';
import { CountsTablePageModule } from './counts-table-page/counts-table-page.module';

const routes: Routes = [
  {
    path: NavigationPath.Counts,
    component: CountsComponent,
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: CountsNavigationPath.Table,
      },
      {
        path: CountsNavigationPath.Table,
        component: CountsTablePageComponent,
      },
      {
        path: CountsNavigationPath.Launcher,
        component: CountsLauncherComponent,
        canActivate: [OperationsGuard],
        data: { operations: [Operation.CreateCounts] } },
    ],
  },
];

@NgModule({
  declarations: [
    CountsComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    CountsTablePageModule,
    CountsLauncherModule,
    TranslateModule,
  ],
})
export class CountsModule {}
