import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { ProductApiSelectModule } from '@cc/common/forms';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuInputDisplayerModule } from '@lucca-front/ng/input';
import {
  LuForOptionsModule,
  LuOptionFeederModule,
  LuOptionItemModule,
  LuOptionPickerModule,
  LuOptionSelectAllModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { TheoreticalMonthRebateModule } from '../../../../../../../common';
import { EndReasonSelectModule } from '../end-reason-select/end-reason-select.module';
import { AttachmentDeletionModalComponent } from './deletion-modal/attachment-deletion-modal.component';
import { AttachmentEndEditionModalComponent } from './end-edition-modal/attachment-end-edition-modal.component';
import { AttachmentExclusionModalComponent } from './exclusion-modal/attachment-exclusion-modal.component';
import { AttachmentLinkingModalComponent } from './linking-modal/attachment-linking-modal.component';
import {
  AttachmentStartEditionModalComponent,
} from './start-edition-modal/attachment-start-edition-modal.component';

@NgModule({
  declarations: [
    AttachmentDeletionModalComponent,
    AttachmentEndEditionModalComponent,
    AttachmentExclusionModalComponent,
    AttachmentLinkingModalComponent,
    AttachmentStartEditionModalComponent,
  ],
  imports: [
    TranslateModule,
    CommonModule,
    ReactiveFormsModule,
    LuDateSelectInputModule,
    EndReasonSelectModule,
    TheoreticalMonthRebateModule,
    ProductApiSelectModule,
    LuSelectInputModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuOptionItemModule,
    LuTooltipTriggerModule,
    LuOptionFeederModule,
    LuOptionSelectAllModule,
    LuForOptionsModule,

  ],
})
export class EstablishmentActionModalsModule { }
