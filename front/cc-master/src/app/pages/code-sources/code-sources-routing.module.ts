import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Operation, OperationsGuard } from '@cc/aspects/rights';
import { NavigationPath } from '@cc/common/navigation';

import { CodeSourcesComponent } from './code-sources.component';
import { CodeSourceCreationEntryModalComponent } from './components/code-source-creation-modal/code-source-creation-modal.component';
import { CodeSourceEditionEntryModalComponent } from './components/code-source-edition-modal/code-source-edition-modal.component';

const routes: Routes = [
  {
    path: NavigationPath.CodeSources,
    component: CodeSourcesComponent,
    children: [
      {
        path: 'create',
        component: CodeSourceCreationEntryModalComponent,
        canActivate: [OperationsGuard],
        data: { operations: [Operation.EditCodeSources] },
      },
      {
        path: ':id/edit',
        component: CodeSourceEditionEntryModalComponent,
        canActivate: [OperationsGuard],
        data: { operations: [Operation.EditCodeSources] },
      },
    ],
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
  ],
  exports: [RouterModule],
})
export class CodeSourcesRoutingModule { }
