import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';

import { ContractEstablishmentHealth, contractEstablishmentsHealth } from '../../constants/contract-establishment-health.enum';
import { IContractEstablishmentHealth } from '../../models/contract-establishment-health.interface';

@Component({
  selector: 'cc-establishment-health-select',
  templateUrl: './establishment-health-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EstablishmentHealthSelectComponent),
      multi: true,
    },
  ],
})
export class EstablishmentHealthSelectComponent implements ControlValueAccessor {
  @Input() public textfieldClass?: string;

  public onChange: (state: IContractEstablishmentHealth) => void;
  public onTouch: () => void;

  public establishmentHealth: IContractEstablishmentHealth;
  public get establishmentsHealth(): IContractEstablishmentHealth[] {
    return this.getTranslatedEstablishmentsHealth(contractEstablishmentsHealth);
  }

  constructor(private translatePipe: TranslatePipe) {}

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(establishmentHealth: IContractEstablishmentHealth): void {
    if (!!establishmentHealth && establishmentHealth !== this.establishmentHealth) {
      this.establishmentHealth = this.getTranslatedEstablishmentsHealth([establishmentHealth])[0];
    }
  }

  public update(): void {
    this.onChange(this.establishmentHealth);
  }

  public trackBy(index: number, establishmentHealth: IContractEstablishmentHealth): number {
    return establishmentHealth.id;
  }

  private getTranslatedEstablishmentsHealth(establishmentsHealth: IContractEstablishmentHealth[]): IContractEstablishmentHealth[] {
    return establishmentsHealth.map(e => {
      const translatedName = e.id === ContractEstablishmentHealth.Error
        ? this.translatePipe.transform('front_select_establishmentHealth_error')
        : this.translatePipe.transform(e.name);

      return { ...e, name: translatedName };
    });
  }
}
