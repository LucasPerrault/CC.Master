import {NgModule} from '@angular/core';
import {ForbiddenComponent} from '@cc/common/errors/forbidden/forbidden.component';
import {CommonModule} from '@angular/common';

@NgModule({
  declarations: [ForbiddenComponent],
  imports: [
    CommonModule
  ],
  providers: []
})
export class ErrorsModule { }
