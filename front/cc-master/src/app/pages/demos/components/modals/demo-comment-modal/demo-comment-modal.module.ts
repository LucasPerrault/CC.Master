import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { DemoCommentModalComponent } from './demo-comment-modal.component';

@NgModule({
  declarations: [DemoCommentModalComponent],
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
})
export class DemoCommentModalModule { }
