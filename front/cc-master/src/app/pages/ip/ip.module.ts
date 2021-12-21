import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { NoNavPath } from '@cc/common/routing';

import { IpComponent } from './ip.component';
import { IpConfirmComponent } from './ip-confirm/ip-confirm.component';
import { IpDataService } from './ip-data.service';
import { IpRejectComponent } from './ip-reject/ip-reject.component';
import { IpRequestComponent } from './ip-request/ip-request.component';

const routes: Routes = [
  {
    path: NoNavPath.Ip,
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: NoNavPath.IpRequest,
      },
      {
        path: NoNavPath.IpRequest,
        component: IpRequestComponent,
      },
      {
        path: NoNavPath.IpConfirm,
        component: IpConfirmComponent,
      },
      {
        path: NoNavPath.IpReject,
        component: IpRejectComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [
    IpComponent,
    IpRequestComponent,
    IpRejectComponent,
    IpConfirmComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
    TranslateModule,
  ],
  providers: [IpDataService],
})
export class IpModule {}
