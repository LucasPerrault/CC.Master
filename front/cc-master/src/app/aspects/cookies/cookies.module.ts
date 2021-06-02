import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { CookiesService } from './cookies.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [CookiesService],
})
export class CookiesModule { }
