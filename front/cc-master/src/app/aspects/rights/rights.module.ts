import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { AnyOperationsGuard } from './guards/any-operations.guard';
import { OperationsGuard } from './guards/operations.guard';
import { RightsService } from './rights.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [
    RightsService,
    OperationsGuard,
    AnyOperationsGuard,
  ],
})
export class RightsModule { }
