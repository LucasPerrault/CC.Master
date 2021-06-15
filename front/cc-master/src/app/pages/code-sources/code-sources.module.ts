import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { CodeSourcesComponent } from './code-sources.component';
import { CodeSourcesListService } from './services/code-sources-list.service';

@NgModule({
  declarations: [CodeSourcesComponent],
  imports: [
    CommonModule,
  ],
  providers: [
    CodeSourcesListService,
  ],
})
export class CodeSourcesModule { }
