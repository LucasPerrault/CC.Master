import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { SelectDisplayMode } from '@cc/common/forms';
import { IEnvironment } from '@cc/domain/environments';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IEnvironmentDetailed } from '../../models/environment-detailed.interface';
import { IPreviousContractEnvironment } from '../../models/previous-contract-environment.interface';


@Component({
  selector: 'cc-environment-link-select',
  templateUrl: './environment-link-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: EnvironmentLinkSelectComponent,
    },
  ],
})
export class EnvironmentLinkSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input() public environmentsSuggested: IEnvironmentDetailed[];

  public onChange: (environment: IEnvironment) => void;
  public onTouch: () => void;

  public selectDisplayMode = SelectDisplayMode;
  public previousContractsEnvironment: IPreviousContractEnvironment[];

  public formControl: FormControl = new FormControl(null, Validators.required);

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {}

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(f => this.onChange(f));
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

  public writeValue(environment: IEnvironment): void {
    if (!!environment && this.formControl.value !== environment) {
      this.formControl.patchValue(environment, { emitEvent: false });
    }
  }

  public suggest(environment: IEnvironment): void {
    this.formControl.setValue(environment);
  }
}
