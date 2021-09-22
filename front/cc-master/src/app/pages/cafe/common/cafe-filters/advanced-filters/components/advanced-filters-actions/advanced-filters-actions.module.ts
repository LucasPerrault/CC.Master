import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { AdvancedFiltersActionsComponent } from './advanced-filters-actions.component';
import { LuDropdownItemModule, LuDropdownPanelModule, LuDropdownTriggerModule } from '@lucca-front/ng/dropdown';
import { TranslateModule } from '@cc/aspects/translate';

@NgModule({
  declarations: [
    AdvancedFiltersActionsComponent,
  ],
    imports: [
        CommonModule,
        LuDropdownTriggerModule,
        LuDropdownPanelModule,
        LuDropdownItemModule,
        TranslateModule,
    ],
  exports: [
    AdvancedFiltersActionsComponent
  ]
})
export class AdvancedFiltersActionsModule { }
