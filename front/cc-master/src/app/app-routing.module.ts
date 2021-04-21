import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OperationsPageGuard } from '@cc/aspects/rights';
import { ForbiddenComponent, NotFoundComponent } from '@cc/common/error-redirections';
import { NavigationPath } from '@cc/common/navigation';
import { NoNavPath } from '@cc/common/routing';

import { LogsComponent } from './pages/logs/logs.component';
import { LogsModule } from './pages/logs/logs.module';

const pageRoutes: Routes = [
  {
    path: NavigationPath.Logs,
    component: LogsComponent,
  },
];

const routes: Routes = [
  { path: NoNavPath.Forbidden, component: ForbiddenComponent },
  { path: NoNavPath.NotFound, component: NotFoundComponent },
  { path: '', canActivate: [OperationsPageGuard], children: pageRoutes },
  { path: '**', redirectTo: NoNavPath.NotFound, pathMatch: 'full' },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    LogsModule,
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }
