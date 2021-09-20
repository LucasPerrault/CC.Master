import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';

import { contractStates } from '../../constants/contract-state.enum';
import { IContractState } from '../../models/contract-state.interface';

@Component({
  selector: 'cc-contract-state-select',
  templateUrl: './contract-state-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContractStateSelectComponent),
      multi: true,
    },
  ],
})
export class ContractStateSelectComponent implements ControlValueAccessor {
  @Input() public textfieldClass?: string;

  public states: IContractState[] = [];
  public get contractStates(): IContractState[] {
    return this.getTranslatedStates(contractStates);
  }

  public onChange: (states: IContractState[]) => void;
  public onTouch: () => void;

  constructor(private translatePipe: TranslatePipe) {}

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(states: IContractState[]): void {
    if (!!states && this.states !== states) {
      this.states = this.getTranslatedStates(states);
    }
  }

  public update(): void {
    this.onChange(this.states);
  }

  public trackBy(index: number, state: IContractState): number {
    return state.id;
  }

  public getTranslatedStates(states: IContractState[]): IContractState[] {
    return states.map(s => ({
      ...s,
      name: this.translatePipe.transform(s.name),
    }));
  }

  public getStateSelectedInRow(values: IContractState[]): string {
    return values.map(v => v.name).join(', ');
  }
}
