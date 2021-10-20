import { Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { SelectDisplayMode } from '@cc/common/forms';
import { EndDateGranularityPolicy, IDateRangeConfiguration } from '@cc/common/forms/select/date-range-select';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DistributorFilter } from '../../../common/distributor-filter-button-group';
import { ContractState, contractStates } from '../../constants/contract-state.enum';
import { IContractState } from '../../models/contract-state.interface';
import { IContractsFilter } from '../../models/contracts-filter.interface';

enum ContractFilterKey {
  Id = 'ids',
  Name = 'name',
  IsDirectSales = 'distributorFilter',
  Clients = 'clients',
  Distributors = 'distributors',
  Products = 'products',
  Offers = 'offers',
  Environments = 'environments',
  Establishments = 'establishments',
  States = 'states',
  EstablishmentHealth = 'establishmentHealth',
  CreatedAt = 'createdAt',
  PeriodOn = 'periodOn',

  StartDate = 'startDate',
  EndDate = 'endDate',
}

@Component({
  selector: 'cc-contracts-manage-filter',
  templateUrl: './contracts-manage-filter.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContractsManageFilterComponent),
      multi: true,
    },
  ],
})
export class ContractsManageFilterComponent implements ControlValueAccessor, OnInit, OnDestroy {

  public onChange: (contractsFilter: IContractsFilter) => void;
  public onTouch: () => void;

  public filtersFormGroup: FormGroup;
  public filterKey = ContractFilterKey;
  public selectMode = SelectDisplayMode;
  public showAdditionalFilters = false;

  public dateRangeConfiguration: IDateRangeConfiguration = {
    granularity: ELuDateGranularity.month,
    periodCoverStrategy: EndDateGranularityPolicy.End,
    startDateConfiguration: { class: 'palette-grey mod-outlined mod-inline mod-long' },
    endDateConfiguration: { class: 'palette-grey mod-outlined mod-inline mod-long' },
  };

  private readonly additionalFilters = [
    ContractFilterKey.Products,
    ContractFilterKey.Offers,
    ContractFilterKey.Environments,
    ContractFilterKey.Establishments,
    ContractFilterKey.States,
    ContractFilterKey.EstablishmentHealth,
    ContractFilterKey.CreatedAt,
    ContractFilterKey.StartDate,
    ContractFilterKey.EndDate,
  ];

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.filtersFormGroup = new FormGroup({
      [ContractFilterKey.Id]: new FormControl(null),
      [ContractFilterKey.Name]: new FormControl(null),
      [ContractFilterKey.IsDirectSales]: new FormControl(DistributorFilter.All),
      [ContractFilterKey.Clients]: new FormControl([]),
      [ContractFilterKey.Distributors] : new FormControl([]),
      [ContractFilterKey.Products]: new FormControl([]),
      [ContractFilterKey.Offers]: new FormControl([]),
      [ContractFilterKey.Environments]: new FormControl([]),
      [ContractFilterKey.Establishments]: new FormControl([]),
      [ContractFilterKey.States]: new FormControl(this.defaultStates),
      [ContractFilterKey.EstablishmentHealth]: new FormControl(null),
      [ContractFilterKey.CreatedAt]: new FormControl(null),
      [ContractFilterKey.PeriodOn]: new FormControl({
        [ContractFilterKey.StartDate]: null,
        [ContractFilterKey.EndDate]: null,
      }),
    });
  }

  public ngOnInit(): void {
    this.filtersFormGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(filters => this.onChange(filters));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private get defaultStates(): IContractState[] {
    return contractStates.filter(s => s.id === ContractState.NotStarted || s.id === ContractState.InProgress);
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(contractsFilter: IContractsFilter): void {
    if (!contractsFilter) {
      return;
    }

    this.filtersFormGroup.setValue(contractsFilter, { emitEvent: false });

    if (this.hasAdditionalFiltersSelected() && !this.hasOnlyDefaultContractStates()) {
      this.toggleMoreFiltersDisplay();
    }
  }

  public toggleMoreFiltersDisplay(): void {
    this.showAdditionalFilters = !this.showAdditionalFilters;
  }

  public getAdditionalFiltersKeySelected(): ContractFilterKey[] {
    return this.additionalFilters.filter(filterKey => {
      if (filterKey === ContractFilterKey.EstablishmentHealth || filterKey === ContractFilterKey.CreatedAt) {
        return !!this.filtersFormGroup.get(filterKey).value;
      }

      if (filterKey === ContractFilterKey.StartDate) {
        const periodOn = this.filtersFormGroup.get(ContractFilterKey.PeriodOn).value;
        return !!periodOn[ContractFilterKey.StartDate];
      }

      if (filterKey === ContractFilterKey.EndDate) {
        const periodOn = this.filtersFormGroup.get(ContractFilterKey.PeriodOn).value;
        return !!periodOn[ContractFilterKey.EndDate];
      }

      return !!this.filtersFormGroup.get(filterKey).value?.length;
    });
  }

  public hasAdditionalFiltersSelected(): boolean {
    return !!this.getAdditionalFiltersKeySelected().length;
  }

  private hasOnlyDefaultContractStates(): boolean {
    const additionalFiltersKey = this.getAdditionalFiltersKeySelected();
    const hasOnlyContractStateKey = additionalFiltersKey.length === 1 && additionalFiltersKey.includes(ContractFilterKey.States);

    return hasOnlyContractStateKey ? this.isDefaultContractStates() : false;
  }

  private isDefaultContractStates(): boolean {
    const states = this.filtersFormGroup.get(ContractFilterKey.States).value;
    if (!states || states.length !== this.defaultStates.length) {
      return false;
    }

    const statesWithoutDefault = states.filter(state => !this.defaultStates.includes(state));
    return !statesWithoutDefault.length;
  }
}
