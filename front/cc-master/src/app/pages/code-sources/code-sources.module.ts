import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CodeSourcesComponent } from './code-sources.component';
import { CodeSourcesListComponent } from './components/code-sources-list/code-sources-list.component';
import { CodeSourcesListService } from './services/code-sources-list.service';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [CodeSourcesComponent, CodeSourcesListComponent],
  imports: [
    CommonModule,
    TranslateModule,
    LuTooltipTriggerModule,
    RouterModule,
  ],
  providers: [
    CodeSourcesListService,
  ],
})
export class CodeSourcesModule { }
