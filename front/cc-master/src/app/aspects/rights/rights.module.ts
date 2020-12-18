import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RightsService} from '@cc/aspects/rights/services/rights.service';
import {OperationsGuard} from '@cc/aspects/rights/guards/operations.guard';

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ],
  providers: [
    RightsService,
    OperationsGuard,
  ]
})
export class RightsModule { }
