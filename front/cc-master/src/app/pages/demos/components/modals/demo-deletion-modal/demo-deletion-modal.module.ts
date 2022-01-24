import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { DemoDeletionModalComponent } from './demo-deletion-modal.component';

@NgModule({
  declarations: [DemoDeletionModalComponent],
  imports: [CommonModule, TranslateModule],
})
export class DemoDeletionModalModule { }
