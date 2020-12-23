import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ForbiddenComponent } from '@cc/common/errors/forbidden/forbidden.component';

@NgModule({
  declarations: [ForbiddenComponent],
  imports: [
    CommonModule,
  ],
  providers: [],
})
export class ErrorsModule { }
