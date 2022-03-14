import { A11yModule } from '@angular/cdk/a11y';
import { OverlayModule } from '@angular/cdk/overlay';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CalendlyComponent, CalendlyModalComponent } from '@cc/common/calendly/components';
import { AccountManagersDataService } from '@cc/common/calendly/services/account-managers/account-managers-data.service';
import { CalendlyService } from '@cc/common/calendly/services/calendly.service';
import { CalendlyModalService } from '@cc/common/calendly/services/calendly-modal.service';
import { LuccaModalHandler, LuccaOverlay, LuccaOverlayContainer } from '@cc/common/calendly/services/lucca-modal';

@NgModule({
  declarations: [CalendlyComponent, CalendlyModalComponent],
  imports: [
    CommonModule,
    OverlayModule,
    A11yModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  exports: [CalendlyComponent],
  providers: [
    LuccaModalHandler,
    LuccaOverlayContainer,
    LuccaOverlay,
    CalendlyService,
    CalendlyModalService,
    AccountManagersDataService,
  ],
})
export class CalendlyModule { }
