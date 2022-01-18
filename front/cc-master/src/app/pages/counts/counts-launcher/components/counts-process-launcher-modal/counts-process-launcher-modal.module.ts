import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { DateRangeSelectModule } from '@cc/common/forms';

import { CountsProcessLauncherModalComponent } from './counts-process-launcher-modal.component';

@NgModule({
  declarations: [
    CountsProcessLauncherModalComponent,
  ],
  imports: [
    CommonModule,
    DateRangeSelectModule,
    ReactiveFormsModule,
  ],
})
export class CountsProcessLauncherModalModule { }
