import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IDistributor } from '@cc/domain/billing/distributors/v4';
import { ALuApiService } from '@lucca-front/ng/api';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DistributorApiSelectService } from './distributor-api-select.service';

@Component({
  selector: 'cc-distributor-api-select',
  templateUrl: './distributor-api-select.component.html',
  styleUrls: ['./distributors-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: ALuApiService,
      useClass: DistributorApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DistributorApiSelectComponent),
      multi: true,
    },
  ],
})
export class DistributorApiSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() required = false;
  @Input() textfieldClass?: string;
  @Input() multiple = false;
  @Input() placeholder: string;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public formControl: FormControl = new FormControl();

  public api = 'api/distributors';

  private destroy$ = new Subject<void>();

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(v => this.onChange(v));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (d: IDistributor | IDistributor[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(d: IDistributor | IDistributor[]): void {
    if (d !== null && d !== this.formControl.value) {
      this.formControl.setValue(d);
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.formControl.disable();
      return;
    }
    this.formControl.enable();
  }

  public trackBy(index: number, distributor: IDistributor): number {
    return distributor.id;
  }

  public getDistributorName(distributor: IDistributor): string {
    const codeAndName = distributor.name;
    const codeAndNameSeparator = '-';
    const separatorIndex = codeAndName.indexOf(codeAndNameSeparator);
    return codeAndName.slice(separatorIndex + 1);
  }
}
