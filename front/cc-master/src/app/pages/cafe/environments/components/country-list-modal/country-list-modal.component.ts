import { Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';

import { IEnvironment } from '../../models/environment.interface';
import { ICountry } from '../../models/legal-unit.interface';

@Component({
  selector: 'cc-country-list-modal',
  templateUrl: './country-list-modal.component.html',
})
export class CountryListModalComponent implements ILuModalContent {
  public title = this.translatePipe.transform('cafe_countries_modal_title');

  public get countries(): ICountry[] {
    return this.environment.legalUnits.map(l => l.country) || [];
  }

  constructor(
    @Inject(LU_MODAL_DATA) public environment: IEnvironment,
    private translatePipe: TranslatePipe,
  ) { }
}
