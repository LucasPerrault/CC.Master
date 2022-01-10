import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ReplaySubject, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { IListEntry, LifecycleStep, ListEntryType } from '../../models/establishment-list-entry.interface';
import { IEstablishmentTypeFilterInfo } from './establiment-type-filter/establishment-type-filter.component';

export enum EstablishmentFilterKey {
  Type = 'type',
  ShowFinishedAttachments = 'showFinishedAttachments',
}

export interface IEstablishmentFilterForm {
  type: ListEntryType;
  showFinishedAttachments: boolean;
}

@Component({
  selector: 'cc-establishment-filters',
  templateUrl: './establishment-filters.component.html',
  styleUrls: ['./establishment-filters.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EstablishmentFiltersComponent),
      multi: true,
    },
  ],
})
export class EstablishmentFiltersComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public allEntriesExceptWithError: IListEntry[];
  @Input() public set hasFinishedEntries(hasFinishedEntries: boolean) {
    this.setShowFinishedAttachmentDisabledState(!hasFinishedEntries);
  }

  public formGroup: FormGroup;
  public formKey = EstablishmentFilterKey;
  public type = ListEntryType;

  public typeFilterInfos$: ReplaySubject<IEstablishmentTypeFilterInfo> = new ReplaySubject(1);

  private destroy$: Subject<void> = new Subject();

  constructor() {
    this.formGroup = new FormGroup({
      [EstablishmentFilterKey.Type]: new FormControl(),
      [EstablishmentFilterKey.ShowFinishedAttachments]: new FormControl(),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));

    this.formGroup.get(EstablishmentFilterKey.ShowFinishedAttachments).valueChanges
      .pipe(takeUntil(this.destroy$), map((show: boolean) => this.getTypeFilterInfos(show)))
      .subscribe(this.typeFilterInfos$);

    this.formGroup.get(EstablishmentFilterKey.Type).valueChanges
      .pipe(takeUntil(this.destroy$), filter(type => !this.hasFinishedAttachments(type)))
      .subscribe(() => this.formGroup.get(EstablishmentFilterKey.ShowFinishedAttachments).reset());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (e: IEstablishmentFilterForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(e: IEstablishmentFilterForm): void {
    if (!!e && e !== this.formGroup.value) {
      this.formGroup.patchValue(e);
    }
  }

  private setShowFinishedAttachmentDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.formGroup.get(EstablishmentFilterKey.ShowFinishedAttachments).disable();
      return;
    }
    this.formGroup.get(EstablishmentFilterKey.ShowFinishedAttachments).enable();
  }

  private getTypeFilterInfos(showFinishedAttachments: boolean): IEstablishmentTypeFilterInfo {
    const entries = showFinishedAttachments
      ? this.allEntriesExceptWithError
      : this.allEntriesExceptWithError.filter(e => e.lifecycleStep !== LifecycleStep.Finished);

    return {
      linkedToThisContractCount: entries.filter(e => e.type === ListEntryType.LinkedToThisContract)?.length,
      linkedToAnotherContractCount: entries.filter(e => e.type === ListEntryType.LinkedToAnotherContract)?.length,
      excludedCount: entries.filter(e => e.type === ListEntryType.Excluded)?.length,
    };
  }

  private hasFinishedAttachments(type: ListEntryType): boolean {
    const entries = this.allEntriesExceptWithError.filter(e => e.type === type);
    return entries.some(e => e.lifecycleStep === LifecycleStep.Finished);
  }
}
