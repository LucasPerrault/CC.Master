import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { CountsLauncherComponent } from './counts-launcher.component';
import { CountsPageTemplateModule } from '../common/counts-page-template/counts-page-template.module';

@NgModule({
  declarations: [CountsLauncherComponent],
  imports: [CommonModule, CountsPageTemplateModule],
})
export class CountsLauncherModule { }
