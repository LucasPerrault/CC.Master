import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CreationCause } from '../../constants/creation-cause.enum';
import { IPreviousContractEnvironment } from '../../models/previous-contract-environment.interface';

@Component({
  selector: 'cc-environment-creation-cause-select',
  templateUrl: './environment-creation-cause-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: EnvironmentCreationCauseSelectComponent,
    },
  ],
})
export class EnvironmentCreationCauseSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input() public previousContracts: IPreviousContractEnvironment[];

  public onChange: (creationCause: CreationCause) => void;
  public onTouch: () => void;

  public formControl: FormControl = new FormControl();
  public creationCause = CreationCause;

  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(creationCause => this.onChange(creationCause));
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

  public writeValue(creationCause: CreationCause): void {
    this.formControl.setValue(creationCause, { emitEvent: false });
  }

}
