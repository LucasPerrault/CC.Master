import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { ErrorRedirectionsModule } from '@cc/common/error-redirections';

import { ErrorNotFoundTabComponent } from './error-not-found-tab.component';

@NgModule({
  declarations: [ErrorNotFoundTabComponent],
  imports: [
    CommonModule,
    ErrorRedirectionsModule,
    TranslateModule,
  ],
})
export class ErrorNotFoundTabModule { }
