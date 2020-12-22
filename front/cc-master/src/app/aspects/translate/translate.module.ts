import { ModuleWithProviders, NgModule } from '@angular/core';
import {
  TranslateLoader as NgxTranslateLoader,
  TranslateModule as NGXTranslateModule,
} from '@ngx-translate/core';

import { TranslateLoader } from './translate.loader';
import { TranslatePipe } from './translate.pipe';
import { TranslateInitializationService } from './translate-initialization.service';

@NgModule({
  imports: [
    NGXTranslateModule.forRoot({ loader: { provide: NgxTranslateLoader, useClass: TranslateLoader } }),
  ],
  providers: [
    TranslateInitializationService,
  ],
})
export class TranslateRootModule {
}

@NgModule({
  imports: [NGXTranslateModule],
  providers: [TranslateInitializationService, TranslatePipe],
  exports: [TranslatePipe],
  declarations: [TranslatePipe],
})
export class TranslateModule {
  static forRoot(): ModuleWithProviders<TranslateRootModule> {
    return { ngModule: TranslateRootModule };
  }
}
