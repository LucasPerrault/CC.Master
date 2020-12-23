import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { ForbiddenComponent } from '@cc/common/error-redirections/forbidden/forbidden.component';

@NgModule({
  declarations: [ForbiddenComponent],
  imports: [
    CommonModule,
    TranslateModule,
  ],
  providers: [],
})
export class ErrorRedirectionsModule { }
