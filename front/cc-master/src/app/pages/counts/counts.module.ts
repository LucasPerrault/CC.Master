import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';

import { CountsComponent } from './counts.component';
import { CountsTableModule } from './counts-table/counts-table.module';

enum ChildrenNavigationPath {
  CountsTable = 'table',
  CountsLauncher = 'launcher'
}

const routes: Routes = [
  {
    path: NavigationPath.Counts,
    component: CountsComponent,
    children: [],
  },
];

@NgModule({
  declarations: [
    CountsComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    CountsTableModule,
    TranslateModule,
  ],
})
export class CountsModule {}
