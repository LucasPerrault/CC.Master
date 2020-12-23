import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { OperationsGuard } from '@cc/aspects/rights/guards/operations.guard';
import { RightsService } from '@cc/aspects/rights/rights.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [
    RightsService,
    OperationsGuard,
  ],
})
export class RightsModule { }
