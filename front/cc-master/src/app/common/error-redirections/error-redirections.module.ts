import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { ForbiddenComponent } from '@cc/common/error-redirections/forbidden/forbidden.component';
import { NotFoundComponent } from '@cc/common/error-redirections/not-found/not-found.component';

@NgModule({
  declarations: [ForbiddenComponent, NotFoundComponent],
  imports: [
    CommonModule,
    TranslateModule,
  ],
  providers: [],
})
export class ErrorRedirectionsModule { }
