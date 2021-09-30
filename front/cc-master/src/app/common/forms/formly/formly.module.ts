import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { EnvironmentApiSelectModule, EnvironmentDomainSelectModule } from '@cc/common/forms';
import { FormlyModule } from '@ngx-formly/core';

import { FormlyFieldEnvironmentDomain } from './environment-domain/environment-domain.component';
import { FormlyFieldEnvironmentSubdomain } from './environment-subdomain/environment-subdomain.component';
import { luFormlyConfig } from './formly.config';

@NgModule({
  declarations: [
    FormlyFieldEnvironmentDomain,
    FormlyFieldEnvironmentSubdomain,
  ],
  imports: [
    CommonModule,
    EnvironmentDomainSelectModule,
    EnvironmentApiSelectModule,
    ReactiveFormsModule,
    FormlyModule.forChild(luFormlyConfig),
  ],
})
export class CCFormlyModule {}
