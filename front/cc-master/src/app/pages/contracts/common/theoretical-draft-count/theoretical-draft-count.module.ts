import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { TheoreticalDraftCountComponent } from './theoretical-draft-count.component';

@NgModule({
  declarations: [TheoreticalDraftCountComponent],
    imports: [
        CommonModule,
        FormsModule,
        TranslateModule,
        ReactiveFormsModule,
    ],
  exports: [TheoreticalDraftCountComponent],
})
export class TheoreticalDraftCountModule { }
