import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CodeSourcesComponent } from './code-sources.component';
import { CodeSourcesListComponent } from './components/code-sources-list/code-sources-list.component';
import { LifecycleButtonGroupComponent } from './components/lifecycle-button-group/lifecycle-button-group.component';
import { CodeSourcesListService } from './services/code-sources-list.service';

@NgModule({
  declarations: [CodeSourcesComponent, CodeSourcesListComponent, LifecycleButtonGroupComponent],
  imports: [
    CommonModule,
    TranslateModule,
    LuTooltipTriggerModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  providers: [
    CodeSourcesListService,
  ],
})
export class CodeSourcesModule { }
