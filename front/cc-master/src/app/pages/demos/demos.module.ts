import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { InstancesDuplicationsDataService } from '@cc/domain/instances';
import { LuModalModule } from '@lucca-front/ng/modal';

import { DemoCardModule } from './components/demo-card/demo-card.module';
import { DemoFiltersModule } from './components/demo-filters/demo-filters.module';
import {
  DemoCommentModalModule,
  DemoCreationModalModule,
  DemoDeletionModalModule,
  DemoPasswordEditionModalModule,
} from './components/modals';
import { DemosComponent } from './demos.component';
import { ConnectAsDataService } from './services/connect-as-data.service';
import { DemosApiMappingService } from './services/demos-api-mapping.service';
import { DemosDataService } from './services/demos-data.service';
import { DemosDuplicationStoreService } from './services/demos-duplication-store.service';
import { DemosListService } from './services/demos-list.service';

@NgModule({
  declarations: [DemosComponent],
  imports: [
    CommonModule,
    DemoFiltersModule,
    ReactiveFormsModule,
    DemoCardModule,
    LuModalModule,
    DemoDeletionModalModule,
    DemoCommentModalModule,
    DemoPasswordEditionModalModule,
    DemoCreationModalModule,
    PagingModule,
    TranslateModule,
  ],
  providers: [
    DemosDataService,
    DemosApiMappingService,
    DemosListService,
    ConnectAsDataService,
    DemosDuplicationStoreService,
    InstancesDuplicationsDataService,
  ],
})
export class DemosModule {
}
