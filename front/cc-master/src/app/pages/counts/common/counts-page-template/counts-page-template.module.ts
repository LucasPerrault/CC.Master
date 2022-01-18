import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';

import { CountsPageTemplateComponent } from './counts-page-template.component';

@NgModule({
  declarations: [CountsPageTemplateComponent],
  imports: [
    CommonModule,
    TranslateModule,
    RouterModule,
  ],
  exports: [CountsPageTemplateComponent],
})
export class CountsPageTemplateModule {
}
