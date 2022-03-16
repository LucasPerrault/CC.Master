import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { DistributorApiSelectModule } from '@cc/common/forms';
import { LuApiSelectInputModule } from '@lucca-front/ng/api';
import { LuSidepanelModule } from '@lucca-front/ng/sidepanel';

import { DemoClientSubdomainInputModule } from '../../selects';
import { DemoApiSelectModule } from '../../selects/demo-api-select/demo-api-select.module';
import { DemoCreationEntryModalComponent, DemoCreationModalComponent } from './demo-creation-modal.component';

@NgModule({
  declarations: [DemoCreationModalComponent, DemoCreationEntryModalComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DemoClientSubdomainInputModule,
    DistributorApiSelectModule,
    DemoApiSelectModule,
    LuApiSelectInputModule,
    LuSidepanelModule,
    TranslateModule,
  ],
})
export class DemoCreationModalModule {}
