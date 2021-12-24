import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { DistributorDomainComponent } from './distributor-domain.component';

@NgModule({
  declarations: [
    DistributorDomainComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslateModule,
  ],
  providers: [],
})
export class DistributorDomainModule {}
