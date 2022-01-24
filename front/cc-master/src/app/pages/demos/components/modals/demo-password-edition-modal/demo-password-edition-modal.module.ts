import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { DemoPasswordEditionModalComponent } from './demo-password-edition-modal.component';

@NgModule({
  declarations: [DemoPasswordEditionModalComponent],
  imports: [CommonModule, ReactiveFormsModule],
})
export class DemoPasswordEditionModalModule { }
