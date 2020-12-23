import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { ForbiddenComponent } from '@cc/common/errors/forbidden/forbidden.component';

@NgModule({
  declarations: [ForbiddenComponent],
  imports: [
    CommonModule,
    TranslateModule,
  ],
  providers: [],
})
export class ErrorsModule { }
