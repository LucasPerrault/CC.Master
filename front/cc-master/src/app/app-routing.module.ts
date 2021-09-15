import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OperationsPageGuard } from '@cc/aspects/rights';
import { ForbiddenComponent, NotFoundComponent } from '@cc/common/error-redirections';
import { NavigationPath, navigationTabs } from '@cc/common/navigation';
import { NoNavPath } from '@cc/common/routing';

import { CodeSourcesComponent } from './pages/code-sources/code-sources.component';
import { CodeSourcesModule } from './pages/code-sources/code-sources.module';
import { ContractsComponent } from './pages/contracts/contracts.component';
import { ContractsModule } from './pages/contracts/contracts.module';
import { LogsComponent } from './pages/logs/logs.component';
import { LogsModule } from './pages/logs/logs.module';

const pageRoutes: Routes = [
  {
    path: NavigationPath.Logs,
    component: LogsComponent,
  },
  {
    path: NavigationPath.CodeSources,
    component: CodeSourcesComponent,
  },
  {
    path: NavigationPath.Contracts,
    component: ContractsComponent,
  },
];

const routes: Routes = [
  { path: NoNavPath.Forbidden, component: ForbiddenComponent },
  { path: NoNavPath.NotFound, component: NotFoundComponent },
  { path: '', canActivate: [OperationsPageGuard], data: { tabs: navigationTabs }, children: pageRoutes },
  { path: '**', redirectTo: NoNavPath.NotFound, pathMatch: 'full' },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    LogsModule,
    CodeSourcesModule,
    ContractsModule,
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }
