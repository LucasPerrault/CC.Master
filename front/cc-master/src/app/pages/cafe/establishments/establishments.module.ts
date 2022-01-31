import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { EstablishmentsComponent } from './establishments.component';

@NgModule({
  declarations: [EstablishmentsComponent],
  imports: [
    CommonModule,
  ],
  exports: [EstablishmentsComponent],
})
export class EstablishmentsModule { }
