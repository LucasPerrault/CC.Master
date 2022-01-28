import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { FieldType } from '@ngx-formly/core';
import { DistributorType } from 'src/app/pages/cafe/common/models/environment.interface';

@Component({
  selector: 'cc-formly-field-distributor-type',
  templateUrl: './distributor-type.component.html',
})
export class FormlyFieldDistributorType extends FieldType {
  readonly formControl: FormControl;

  public get distributorTypes(): DistributorType[] {
    return Object
      .keys(DistributorType)
      .filter(k => isNaN(Number(k)))
      .map(k => DistributorType[k]);
  }

  constructor(private translatePipe: TranslatePipe)
  {
    super();
  }

  public onTouch: () => void = () => {};

  public getDistributorTypeTranslation(type: DistributorType): string {
    switch (type) {
      case DistributorType.DirectOnly:
        return this.translatePipe.transform('cafe_environments_select_direct_only');
      case DistributorType.IndirectOnly:
        return this.translatePipe.transform('cafe_environments_select_indirect_only');
      case DistributorType.DirectAndIndirect:
        return this.translatePipe.transform('cafe_environments_select_direct_and_indirect');
    }
  }
}
