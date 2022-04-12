import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';
import { PagingModule } from '@cc/common/paging';
import { InstanceDuplicationsService, InstancesDuplicationsDataService } from '@cc/domain/instances';
import { LuModalModule } from '@lucca-front/ng/modal';

import { DemoCardModule } from './components/demo-card/demo-card.module';
import { DemoPasswordModule } from './components/demo-card/demo-password/demo-password.module';
import { DemoCardPlaceholderModule } from './components/demo-card-placeholder/demo-card-placeholder.module';
import { DemoDuplicationCardModule } from './components/demo-duplication-card/demo-duplication-card.module';
import { DemoFiltersModule } from './components/demo-filters/demo-filters.module';
import {
  DemoCommentModalModule,
  DemoCreationModalModule,
  DemoDeletionModalModule,
} from './components/modals';
import { DemoCreationEntryModalComponent } from './components/modals/demo-creation-modal/demo-creation-modal.component';
import { DemosComponent } from './demos.component';
import { ConnectAsDataService } from './services/connect-as-data.service';
import { DemoDuplicationsService } from './services/demo-duplications.service';
import { DemosApiMappingService } from './services/demos-api-mapping.service';
import { DemosDataService } from './services/demos-data.service';
import { DemosListService } from './services/demos-list.service';

const routes: Routes = [
  {
    path: NavigationPath.Demos,
    component: DemosComponent,
    children: [
      {
        path: 'create',
        component: DemoCreationEntryModalComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [DemosComponent],
  imports: [
    CommonModule,
    DemoFiltersModule,
    ReactiveFormsModule,
    DemoPasswordModule,
    LuModalModule,
    DemoDeletionModalModule,
    DemoCommentModalModule,
    DemoCreationModalModule,
    PagingModule,
    RouterModule.forChild(routes),
    TranslateModule,
    DemoCardPlaceholderModule,
    DemoDuplicationCardModule,
    DemoCardModule,
  ],
  providers: [
    DemosDataService,
    DemosApiMappingService,
    DemosListService,
    ConnectAsDataService,
    InstancesDuplicationsDataService,
    InstanceDuplicationsService,
    DemoDuplicationsService,
  ],
})
export class DemosModule {
}
