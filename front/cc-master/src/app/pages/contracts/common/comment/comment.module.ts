import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { CommentComponent } from './comment.component';

@NgModule({
  declarations: [CommentComponent],
    imports: [
        CommonModule,
        FormsModule,
        TranslateModule,
        ReactiveFormsModule,
    ],
  exports: [CommentComponent],
})
export class CommentModule { }
