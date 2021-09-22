import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormControl,
  FormGroup,
} from '@angular/forms';

import { IAdvancedFilterAndCount } from './models/advanced-filter-and-count.interface';
import { IAdvancedFilterConfiguration } from './models/advanced-filter-configuration.interface';
import { AdvancedFilterApiMappingService } from './services/advanced-filter-api-mapping.service';

enum CriterionFormsKey {
  ComparisonFilterCriterionForm = 'comparisonFilterCriterionForm',
}

@Component({
  selector: 'cc-advanced-filters',
  templateUrl: './advanced-filters.component.html',
  styleUrls: ['./advanced-filters.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdvancedFiltersComponent implements OnInit {
  @Input() public configuration: IAdvancedFilterConfiguration;

  @Output() public updateFilters: EventEmitter<IAdvancedFilterAndCount> = new EventEmitter<IAdvancedFilterAndCount>();
  @Output() public cancel: EventEmitter<void> = new EventEmitter<void>();

  public get invalid(): boolean {
    const hasLogicalOperatorInvalid = this.formArray.length > 1 && this.logicalOperator.invalid;
    return this.formArray.invalid || hasLogicalOperatorInvalid;
  }

  public logicalOperator: FormControl = new FormControl();

  public formArrayKey = 'criterions';
  public formArray: FormArray = new FormArray([]);
  public formGroup: FormGroup = new FormGroup({
    [this.formArrayKey]: this.formArray,
  });

  constructor(private apiMappingService: AdvancedFilterApiMappingService) {
  }

  public ngOnInit(): void {
    this.add();
  }

  public trackBy(index: number, control: AbstractControl): number {
    return index;
  }

  public add(): void {
    this.formArray.push(new FormControl({
      [CriterionFormsKey.ComparisonFilterCriterionForm]: null,
    }));
  }

  public remove(index: number) {
    this.formArray.removeAt(index);
  }

  public clear(): void {
    this.logicalOperator.reset();
    this.formArray.clear();
  }

  public cancelForm(): void {
    this.cancel.emit();
  }

  public submitAdvancedFilter(): void {
    const criterionForms = this.formGroup.get(this.formArrayKey)?.value;
    if (!criterionForms && !!criterionForms.length) {
      return;
    }

    const logicalOperator = this.logicalOperator.value?.id;
    const advancedFilter = this.apiMappingService.toAdvancedFilter(logicalOperator, criterionForms, this.configuration);

    this.updateFilters.emit({
      count: this.formArray.length,
      filter: advancedFilter,
    });
  }
}
