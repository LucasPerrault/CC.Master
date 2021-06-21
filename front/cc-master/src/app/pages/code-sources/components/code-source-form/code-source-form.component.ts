import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CodeSourceFormKey } from '../../constants/code-source-form-key.enum';
import { ICodeSource } from '../../models/code-source.interface';

@Component({
  selector: 'cc-code-source-form',
  templateUrl: './code-source-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CodeSourceFormComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: CodeSourceFormComponent,
    },
  ],
})
export class CodeSourceFormComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public codeSourcesFetched: ICodeSource[];
  @Input() public enabledExceptions: CodeSourceFormKey[] = [];
  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (f: any) => void;
  public onTouch: () => void;

  public formGroup: FormGroup;
  public formGroupKey = CodeSourceFormKey;

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.formGroup = new FormGroup({
      [CodeSourceFormKey.Code]: new FormControl(''),
      [CodeSourceFormKey.Type]: new FormControl(''),
      [CodeSourceFormKey.Lifecycle]: new FormControl(),
      [CodeSourceFormKey.JenkinsProjectName]: new FormControl(''),
      [CodeSourceFormKey.Config]: new FormGroup({
        [CodeSourceFormKey.AppPath]: new FormControl(),
        [CodeSourceFormKey.Subdomain]: new FormControl(),
        [CodeSourceFormKey.IisServerPath]: new FormControl(),
        [CodeSourceFormKey.IsPrivate]: new FormControl(),
      }),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
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

  public writeValue(codeSource: ICodeSource): void {
    if (!!codeSource && codeSource !== this.formGroup.value) {
      this.formGroup.patchValue(codeSource, { emitEvent: false });
    }
  }

  public setDisabledState(isDisabled: boolean) {
    const controlKeys = Object.keys(this.formGroup.controls);
    controlKeys.forEach((key: CodeSourceFormKey) => {
      if (isDisabled && !this.enabledExceptions.includes(key)) {
        this.formGroup.get(key).disable();
        return;
      }

      this.formGroup.get(key).enable();
    });
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return { invalid: true };
    }
  }
}
