import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ICodeSource } from '../../models/code-source.interface';

@Component({
  selector: 'cc-code-source-select',
  templateUrl: './code-source-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CodeSourceSelectComponent),
      multi: true,
    },
  ],
})
export class CodeSourceSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public codeSources: ICodeSource[];
  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (cs: ICodeSource) => void;
  public onTouch: () => void;

  public codeSourceSelected: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.codeSourceSelected.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(cs => this.onChange(cs));
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

  public writeValue(codeSource: ICodeSource): void {
    if (!!codeSource && codeSource !== this.codeSourceSelected.value) {
      this.codeSourceSelected.setValue(codeSource, { emitEvent: false });
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.codeSourceSelected.disable();
      return;
    }
    this.codeSourceSelected.enable();
  }
}
