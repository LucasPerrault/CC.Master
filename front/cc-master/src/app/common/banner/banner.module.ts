import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CookiesModule } from '@cc/aspects/cookies';
import { TranslateModule } from '@cc/aspects/translate';

import { BannerComponent } from './banner.component';

@NgModule({
  declarations: [BannerComponent],
  imports: [
    CommonModule,
    CookiesModule,
    TranslateModule,
  ],
})
export class BannerModule { }
