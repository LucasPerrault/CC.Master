import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { CountsTablePageComponent } from './counts-table-page.component';
import { CountsPageTemplateModule } from '../common/counts-page-template/counts-page-template.module';
import { CountsListComponent } from './components/counts-list/counts-list.component';
import { TranslateModule } from '@cc/aspects/translate';

@NgModule({
  declarations: [
    CountsTablePageComponent,
    CountsListComponent,
  ],
  imports: [
    CommonModule,
    CountsPageTemplateModule,
    TranslateModule,
  ],
})
export class CountsTablePageModule { }
