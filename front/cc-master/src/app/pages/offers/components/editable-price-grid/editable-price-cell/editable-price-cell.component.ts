import { Component, EventEmitter, forwardRef, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, NG_VALIDATORS, NG_VALUE_ACCESSOR, ValidationErrors } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ArrowKey } from '../editable-price-grid.component';

@Component({
  selector: 'cc-editable-price-cell',
  templateUrl: './editable-price-cell.component.html',
  styleUrls: ['./editable-price-cell.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EditablePriceCellComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: EditablePriceCellComponent,
    },
  ],
})
export class EditablePriceCellComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Output() public keydownChange: EventEmitter<ArrowKey> = new EventEmitter<ArrowKey>();
  @Output() public pasteChange: EventEmitter<ClipboardEvent> = new EventEmitter<ClipboardEvent>();
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }
  @Input() public readonly = false;

  public formControl: FormControl = new FormControl();

  public get invalid(): boolean {
    return this.formControl.invalid && this.formControl.touched;
  }

  private destroy$: Subject<void> = new Subject();

  private readonly isEventPropagated = false;

  constructor() { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(price => this.onChange(price));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (p: number) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(price: number): void {
    this.formControl.setValue(price);
  }

  public setDisabledState(isDisabled: boolean): void {
    if (isDisabled) {
      this.formControl.disable();
      this.readonly = true;
      return;
    }

    this.formControl.enable();
    this.readonly = false;
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public keydownLeft(): boolean {
    this.keydownChange.emit(ArrowKey.Left);
    return this.isEventPropagated;
  }

  public keydownRight(): boolean {
    this.keydownChange.emit(ArrowKey.Right);
    return this.isEventPropagated;
  }


  public keydownDown(): boolean {
    this.keydownChange.emit(ArrowKey.Down);
    return this.isEventPropagated;
  }

  public keydownUp(): boolean {
    this.keydownChange.emit(ArrowKey.Up);
    return this.isEventPropagated;
  }

  public paste(event: ClipboardEvent): boolean {
    this.pasteChange.emit(event);
    return this.isEventPropagated;
  }
}
