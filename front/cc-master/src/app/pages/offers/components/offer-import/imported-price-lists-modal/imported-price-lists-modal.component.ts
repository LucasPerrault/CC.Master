import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';

import { IPriceListForm } from '../../../models/price-list-form.interface';
import { PriceListsValidators,PriceListValidationError } from '../../../services/price-lists.validators';

enum ImportedListFormKey {
  StartsOn = 'startsOn',
  PriceRows = 'rows',
}

@Component({
  selector: 'cc-imported-price-lists-modal',
  templateUrl: './imported-price-lists-modal.component.html',
  styleUrls: ['./imported-price-lists-modal.component.scss'],
})
export class ImportedPriceListsModalComponent implements ILuModalContent {
  public title = this.translatePipe.transform('offers_form_imported_priceList_title', { count: this.priceLists?.length });

  public formArrayKey = 'importedPriceLists';
  public formArray: FormArray = new FormArray([], [PriceListsValidators.uniqStartsOnRange]);
  public formKey = ImportedListFormKey;
  public formGroup: FormGroup = new FormGroup(
    { [this.formArrayKey]: this.formArray },
  );
  public granularity = ELuDateGranularity;
  public validationError = PriceListValidationError;

  constructor(
    @Inject(LU_MODAL_DATA) private priceLists: IPriceListForm[],
    private translatePipe: TranslatePipe,
  ) {
    this.addRange(priceLists);
  }

  public trackBy(index: number, list: AbstractControl): AbstractControl {
    return list;
  }

  private addRange(lists: IPriceListForm[]): void {
    lists.forEach(list => this.add(list));
  }

  private add(list: IPriceListForm): void {
    const formGroup = this.getFormGroup(list);
    this.formArray.push(formGroup);
  }

  private getFormGroup(list: IPriceListForm): FormGroup {
    return new FormGroup(
      {
        [ImportedListFormKey.StartsOn]: new FormControl(new Date(list?.startsOn)),
        [ImportedListFormKey.PriceRows]: new FormControl(list?.rows),
      },
    );
  }

}
