import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { lifecycles, LifecycleStep } from '../../constants/lifecycle-step.enum';
import { ILifecycleStep } from '../../models/lifecycle-step.interface';

@Component({
  selector: 'cc-lifecycle-select',
  templateUrl: './lifecycle-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => LifecycleSelectComponent),
      multi: true,
    },
  ],
})
export class LifecycleSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (l: LifecycleStep) => void;
  public onTouch: () => void;

  public lifecycleSelected: FormControl = new FormControl();
  public get translatedLifecycles(): ILifecycleStep[] {
    return this.getTranslatedLifecycles(lifecycles);
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private translatePipe: TranslatePipe) { }

  public ngOnInit(): void {
    this.lifecycleSelected.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(l => this.onChange(l?.id));
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

  public writeValue(lifecycle: LifecycleStep): void {
    const lifecycleStep = this.translatedLifecycles.find(l => l.id === lifecycle);
    if (lifecycleStep !== this.lifecycleSelected.value) {
      this.lifecycleSelected.setValue(lifecycleStep, { emitEvent: false });
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.lifecycleSelected.disable();
      return;
    }
    this.lifecycleSelected.enable();
  }

  public trackBy(index: number, lifecycleStep: ILifecycleStep): string {
    return lifecycleStep.id;
  }

  private getTranslatedLifecycles(ls: ILifecycleStep[]): ILifecycleStep[] {
    return ls.map(lifecycle => ({ ...lifecycle, name: this.translatePipe.transform(lifecycle.name) }));
  }
}
