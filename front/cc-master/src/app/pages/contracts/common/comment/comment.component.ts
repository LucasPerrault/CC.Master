import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
  Validators,
} from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-comment',
  templateUrl: './comment.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CommentComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: CommentComponent,
    },
  ],
})
export class CommentComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public textfieldClass?: string;
  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public comment: FormControl;

  public get maxCommentLength(): number {
    return 4000;
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.comment = new FormControl('', Validators.maxLength(this.maxCommentLength));
  }

  public ngOnInit(): void {
    this.comment.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(comment => this.onChange(comment));

    this.comment.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(s => this.onValidatorChange());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onValidatorChange: () => void = () => {};
  public onChange: (comment: string) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(comment: string): void {
    if (comment !== this.comment.value) {
      this.comment.setValue(comment);
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.comment.disable();
      return;
    }
    this.comment.enable();
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.comment.invalid) {
      return { invalid: true };
    }
  }

  public registerOnValidatorChange(fn: () => void) {
    this.onValidatorChange = fn;
  }

  public get isMaxLengthReached(): boolean {
    return this.charactersRemaining < 0;
  }

  public get charactersRemaining(): number {
    return this.maxCommentLength - this.comment?.value.length;
  }
}
