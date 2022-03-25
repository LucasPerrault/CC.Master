import { Component } from '@angular/core';

import { OffersRoutingService } from './services/offers-routing.service';

@Component({
  selector: 'cc-offers-page',
  template: '<router-outlet></router-outlet>',
})
export class OffersPageComponent {
  constructor(private routingService: OffersRoutingService) {
    this.routingService.setDefaultTag();
  }
}
