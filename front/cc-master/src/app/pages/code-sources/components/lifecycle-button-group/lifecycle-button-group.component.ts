import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { LifecycleStep } from '../../constants/lifecycle-step.enum';
import { ICodeSourcesFiltered } from '../../models/code-sources-filtered.interface';
import { LifecycleStepFilter } from '../../models/lifecycle-step-filter.interface';

@Component({
  selector: 'cc-lifecycle-button-group',
  templateUrl: './lifecycle-button-group.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => LifecycleButtonGroupComponent),
      multi: true,
    },
  ],
})
export class LifecycleButtonGroupComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input() public codeSourcesFiltered: ICodeSourcesFiltered;

  public onChange: (lifecycle: LifecycleStepFilter) => void;
  public onTouch: () => void;

  public lifecycleStep = LifecycleStep;
  public lifecycleStepsSelected: FormControl;

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.lifecycleStepsSelected = new FormControl(null);
  }

  public ngOnInit(): void {
    this.lifecycleStepsSelected.valueChanges
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

  public writeValue(lifecycleStep: LifecycleStepFilter): void {
    if (!!lifecycleStep && this.lifecycleStepsSelected.value !== lifecycleStep) {
      this.lifecycleStepsSelected.setValue(lifecycleStep, { emitEvent: false });
    }
  }
}
