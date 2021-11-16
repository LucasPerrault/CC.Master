import { Component, OnInit } from '@angular/core';
import { IImportedOffer } from './imported-offer.interface';

@Component({
  selector: 'cc-offer-import',
  templateUrl: './offer-import.component.html',
  styleUrls: ['./offer-import.component.scss'],
})
export class OfferImportComponent implements OnInit {

  public offers: IImportedOffer[] = [];

  constructor() { }

  ngOnInit(): void {
  }

}
