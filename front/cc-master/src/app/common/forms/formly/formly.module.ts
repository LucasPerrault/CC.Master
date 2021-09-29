import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { EnvironmentDomainSelectModule } from '@cc/common/forms';
import { FormlyFieldEnvironmentDomain } from '@cc/common/forms/formly/environment-domain/environment-domain.component';
import { FormlyModule } from '@ngx-formly/core';

import { luFormlyConfig } from './formly.config';

@NgModule({
  declarations: [
    FormlyFieldEnvironmentDomain,
  ],
  imports: [
    CommonModule,
    EnvironmentDomainSelectModule,
    ReactiveFormsModule,
    FormlyModule.forChild(luFormlyConfig),
  ],
})
export class CCFormlyModule {}
