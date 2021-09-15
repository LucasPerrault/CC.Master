import { ChangeDetectorRef, Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { IClient } from '@cc/domain/billing/clients';

import { SelectDisplayMode } from '../select-display-mode.enum';

@Component({
  selector: 'cc-client-api-select',
  templateUrl: './client-api-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ClientApiSelectComponent),
      multi: true,
    },
  ],
})
export class ClientApiSelectComponent implements ControlValueAccessor {
  @Input() required = false;
  @Input() displayMode = SelectDisplayMode.Filter;
  @Input() textfieldClass?: string;
  @Input() multiple = false;

  @Input()
  public get disabled(): boolean { return this.isDisabled; }
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (d: IClient | IClient[]) => void;
  public onTouch: () => void;

  public apiUrl = 'api/v3/clients';

  public model: IClient | IClient[] = [];
  public clientsSelected: IClient[];

  public get filtersToExcludeSelection(): string[] {
    if (!this.multiple || !this.clientsSelected?.length) {
      return [];
    }

    const selectedIds = this.clientsSelected.map(e => e.id);
    return [`id=notequal,${selectedIds.join(',')}`];
  }

  private isDisabled = false;

  constructor(private changeDetectorRef: ChangeDetectorRef, private translatePipe: TranslatePipe) {
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(d: IClient | IClient[]): void {
    if (d !== this.model && d !== null) {
      this.model = d;
      this.changeDetectorRef.detectChanges();
    }
  }

  public setDisabledState(isDisabled: boolean) {
    this.isDisabled = isDisabled;
  }

  public update(d: IClient | IClient[]): void {
    this.onChange(d);
  }

  public setClientsDisplayed(): void {
    if (!this.multiple) {
      return;
    }

    this.clientsSelected = (this.model as IClient[]);
  }

  public trackBy(index: number, client: IClient): number {
    return client.id;
  }

  public get label(): string {
    const pluralCaseCount = 2;
    const singleCaseCount = 1;
    return this.translatePipe.transform('front_select_clients_label', {
      count: this.multiple ? pluralCaseCount : singleCaseCount,
    });
  }

  public get isFormDisplayMode(): boolean {
    return this.displayMode === SelectDisplayMode.Form;
  }

  public get placeholder(): string {
    if (this.isFormDisplayMode) {
      return;
    }

    return this.label;
  }
}
