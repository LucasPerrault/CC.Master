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

import { ICodeSource } from '../../models/code-source.interface';

enum CodeSourceFormKey {
  Code = 'code',
  Type = 'type',
  Lifecycle = 'lifecycle',
  JenkinsProjectName = 'jenkinsProjectName',
  Config = 'config',
  AppPath = 'appPath',
  Subdomain = 'subdomain',
  IisServerPath = 'iisServerPath',
  IsPrivate = 'isPrivate',
}

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
  @Input() public set canEditLifecycle(isEnabled: boolean) { this.setLifecycleState(!isEnabled); }
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
    if (isDisabled) {
      this.formGroup.disable();
      return;
    }
    this.formGroup.enable();
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return { invalid: true };
    }
  }

  private setLifecycleState(isDisabled: boolean) {
    if (isDisabled) {
      this.formGroup.get(CodeSourceFormKey.Lifecycle).disable({ emitEvent: false });
      return;
    }

    this.formGroup.get(CodeSourceFormKey.Lifecycle).enable({ emitEvent: false });
  }
}
