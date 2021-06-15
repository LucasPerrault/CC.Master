import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CodeSourcesComponent } from './code-sources.component';
import {
  CodeSourceCreationModalComponent,
  CodeSourcesCreationEntryModalComponent,
} from './components/code-source-creation-modal/code-source-creation-modal.component';
import { CodeSourcesListComponent } from './components/code-sources-list/code-sources-list.component';
import { LifecycleButtonGroupComponent } from './components/lifecycle-button-group/lifecycle-button-group.component';
import { CodeSourcesListService } from './services/code-sources-list.service';
import { LuSidepanelModule } from '@lucca-front/ng/sidepanel';

const routes: Routes = [
  {
    path: NavigationPath.CodeSources,
    component: CodeSourcesComponent,
    children: [
      {
        path: 'create',
        component: CodeSourcesCreationEntryModalComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [
    CodeSourcesComponent,
    CodeSourcesListComponent,
    LifecycleButtonGroupComponent,
    CodeSourcesCreationEntryModalComponent,
    CodeSourceCreationModalComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    TranslateModule,
    LuTooltipTriggerModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    LuSidepanelModule,
  ],
  providers: [
    CodeSourcesListService,
  ],
})
export class CodeSourcesModule { }
