import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AnyOperationsGuard } from '@cc/aspects/rights';
import { ForbiddenComponent, forbiddenUrl, NotFoundComponent, notFoundUrl } from '@cc/common/error-redirections';
import { NavigationPath } from '@cc/common/navigation';

import { LogsComponent } from './pages/logs/logs.component';
import { LogsModule } from './pages/logs/logs.module';

const pageRoutes: Routes = [
  {
    path: NavigationPath.Logs,
    component: LogsComponent,
  },
];

const routes: Routes = [
  { path: forbiddenUrl, component: ForbiddenComponent },
  { path: notFoundUrl, component: NotFoundComponent },
  { path: '', canActivate: [AnyOperationsGuard], children: pageRoutes },
  { path: '**', redirectTo: notFoundUrl, pathMatch: 'full' },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    LogsModule,
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }
