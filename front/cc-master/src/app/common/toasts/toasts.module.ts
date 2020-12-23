import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { ToastsComponent } from './toasts.component';
import { ToastsService } from './toasts.service';

@NgModule({
    declarations: [ToastsComponent],
    imports: [
        CommonModule,
    ],
    exports: [ToastsComponent],
    providers: [ToastsService],
})
export class ToastsModule { }
