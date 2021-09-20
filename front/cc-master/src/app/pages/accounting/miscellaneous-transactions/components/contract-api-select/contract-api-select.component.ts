import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { IContract } from '@cc/domain/billing/contracts/v4';
import { ContractCalculatedProperties } from '@cc/domain/billing/contracts/v4/models/contract-calculated-properties';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-contract-api-select',
  templateUrl: './contract-api-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContractApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ContractApiSelectComponent,
    },
  ],
})
export class ContractApiSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public label: string;
  @Input() public placeholder: string;
  @Input() public multiple = false;
  @Input() public textfieldClass?: string;
  @Input() public required = false;

  public onChange: (contracts: IContract | IContract[]) => void;
  public onTouch: () => void;

  public apiUrl = '/api/contracts';
  public apiFields = 'id,name';

  public formControl: FormControl = new FormControl();
  public selectedContracts: IContract[];

  public get filtersToExcludeSelection(): string[] {
    if (!this.selectedContracts?.length) {
      return [];
    }

    const selectedContractIds = this.selectedContracts.map(e => e.id);
    return [`id=notequal,${ selectedContractIds.join(',') }`];
  }

  private destroy$: Subject<void> = new Subject();

  constructor() {}

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(contracts => this.onChange(contracts));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(contracts: IContract | IContract[]): void {
    if (contracts !== this.selectedContracts) {
      this.formControl.setValue(contracts, { emitEvent: false });
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public getContractName(contract: IContract): string {
    return ContractCalculatedProperties.name(contract);
  }

  public onOpen(): void {
    if (!this.multiple) {
      return;
    }
    this.selectedContracts = this.formControl.value;
  }

  public trackBy(index: number, contract: IContract): number {
    return contract.id;
  }
}
