import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { EstablishmentsComponent } from './establishments.component';
import { EstablishmentsConfiguration } from './establishments.configuration';

@NgModule({
  declarations: [EstablishmentsComponent],
  imports: [
    CommonModule,
  ],
  exports: [EstablishmentsComponent],
  providers: [EstablishmentsConfiguration],
})
export class EstablishmentsModule { }
