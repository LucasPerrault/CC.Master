import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { OperationsGuard } from './guards/operations.guard';
import { OperationsPageGuard } from './guards/operations-page.guard';
import { RightsService } from './rights.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [
    RightsService,
    OperationsGuard,
    OperationsPageGuard,
  ],
})
export class RightsModule { }
