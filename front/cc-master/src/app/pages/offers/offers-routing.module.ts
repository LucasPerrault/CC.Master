import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Operation, OperationsGuard } from '@cc/aspects/rights';
import { NavigationPath } from '@cc/common/navigation';

import { OfferCreationComponent } from './components/offer-creation/offer-creation.component';
import { OfferCreationModule } from './components/offer-creation/offer-creation.module';
import { OfferEditionComponent } from './components/offer-edition/offer-edition.component';
import { OfferEditionModule } from './components/offer-edition/offer-edition.module';
import { OfferEditionNavigationPath } from './components/offer-edition/offer-edition-navigation-tabs.const';
import { OfferEditionTabComponent } from './components/offer-edition/offer-edition-tab/offer-edition-tab.component';
import { OfferNotFoundTabComponent } from './components/offer-edition/offer-not-found-tab/offer-not-found-tab.component';
import { OfferPriceListsTabComponent } from './components/offer-edition/offer-price-lists-tab/offer-price-lists-tab.component';
import { OfferImportComponent } from './components/offer-import/offer-import.component';
import { OfferImportModule } from './components/offer-import/offer-import.module';
import { OffersComponent } from './offers.component';

const routes: Routes = [
  {
    path: NavigationPath.Offers,
    component: OffersComponent,
  },
  {
    path: `${ NavigationPath.Offers }/create`,
    component: OfferCreationComponent,
    canActivate: [OperationsGuard],
    data: { operations: [Operation.CreateCommercialOffers] },
  },
  {
      path: `${ NavigationPath.Offers }/import`,
    component: OfferImportComponent,
    canActivate: [OperationsGuard],
    data: { operations: [Operation.CreateCommercialOffers] },
  },
  {
    path: `${ NavigationPath.Offers }/:id/${ OfferEditionNavigationPath.NotFound }`,
    component: OfferNotFoundTabComponent,
  },
  {
    path: `${ NavigationPath.Offers }/:id`,
    component: OfferEditionComponent,
    canActivate: [OperationsGuard],
    data: { operations: [Operation.ReadCommercialOffers] },
    children: [
      {
        path: OfferEditionNavigationPath.Edit,
        component: OfferEditionTabComponent,
      },
      {
        path: OfferEditionNavigationPath.PriceLists,
        component: OfferPriceListsTabComponent,
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: OfferEditionNavigationPath.Edit,
      },
    ],
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
    OfferEditionModule,
    OfferCreationModule,
    OfferImportModule,
  ],
  exports: [RouterModule],
})
export class OffersRoutingModule { }
