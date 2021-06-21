import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { LuSidepanelModule } from '@lucca-front/ng/sidepanel';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CodeSourcesComponent } from './code-sources.component';
import { CodeSourcesRoutingModule } from './code-sources-routing.module';
import {
  CodeSourceCreationEntryModalComponent,
  CodeSourceCreationModalComponent,
} from './components/code-source-creation-modal/code-source-creation-modal.component';
import {
  CodeSourceEditionEntryModalComponent,
  CodeSourceEditionModalComponent,
} from './components/code-source-edition-modal/code-source-edition-modal.component';
import { CodeSourceFormComponent } from './components/code-source-form/code-source-form.component';
import { CodeSourceSelectModule } from './components/code-source-select/code-source-select.module';
import { CodeSourcesListComponent } from './components/code-sources-list/code-sources-list.component';
import { LifecycleButtonGroupComponent } from './components/lifecycle-button-group/lifecycle-button-group.component';
import { LifecycleSelectModule } from './components/lifecycle-select/lifecycle-select.module';
import { CodeSourcesService } from './services/code-sources.service';
import { CodeSourcesListService } from './services/code-sources-list.service';

@NgModule({
  declarations: [
    CodeSourcesComponent,
    CodeSourcesListComponent,
    LifecycleButtonGroupComponent,
    CodeSourceFormComponent,
    CodeSourceCreationEntryModalComponent,
    CodeSourceCreationModalComponent,
    CodeSourceEditionEntryModalComponent,
    CodeSourceEditionModalComponent,
  ],
  imports: [
    CommonModule,
    CodeSourcesRoutingModule,
    TranslateModule,
    LuTooltipTriggerModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    LuSidepanelModule,
    CodeSourceSelectModule,
    LifecycleSelectModule,
  ],
  providers: [
    CodeSourcesListService,
    CodeSourcesService,
  ],
})
export class CodeSourcesModule { }
