import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Operation, OperationsGuard } from '@cc/aspects/rights';
import { NavigationPath } from '@cc/common/navigation';

import { OfferCreationComponent } from './components/offer-creation/offer-creation.component';
import { OfferCreationModule } from './components/offer-creation/offer-creation.module';
import { OfferEditionComponent } from './components/offer-edition/offer-edition.component';
import { OfferEditionModule } from './components/offer-edition/offer-edition.module';
import { OffersComponent } from './offers.component';

const routes: Routes = [
  {
    path: NavigationPath.Offers,
    component: OffersComponent,
  },
  {
    path: `${ NavigationPath.Offers }/:id/edit`,
    component: OfferEditionComponent,
    canActivate: [OperationsGuard],
    data: { operations: [Operation.CreateCommercialOffers] },
  },
  {
    path: `${ NavigationPath.Offers }/create`,
    component: OfferCreationComponent,
    canActivate: [OperationsGuard],
    data: { operations: [Operation.CreateCommercialOffers] },
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
    OfferEditionModule,
    OfferCreationModule,
  ],
  exports: [RouterModule],
})
export class OffersRoutingModule { }
