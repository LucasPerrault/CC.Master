import { Injectable } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable, pipe, Subject, UnaryFunction } from 'rxjs';
import { debounceTime, map, takeUntil, tap } from 'rxjs/operators';

import { IAdvancedCriterionForm, IAdvancedFilterForm } from '../../components/advanced-filter-form';
import { IFacetAndColumn } from '../../forms/select/facets-and-columns-api-select';
import { IAutoSelectedColumnMapping,IAutoSelectedColumnState } from './auto-selected-column-mapping.interface';
import { IAutoSelectedCriterionForm } from './auto-selected-criterion.interface';

interface IColumnAutoSelectionData {
  selection: IAutoSelectedCriterionForm;
  allCriterions: IAdvancedCriterionForm[];
}

@Injectable()
export class ColumnAutoSelectionService {
  public create(
    advancedFilterFormControl: FormControl,
    columnsFormControl: FormControl,
    mappings: IAutoSelectedColumnMapping[],
    destroy$: Subject<void>,
  ): AdvancedFilterColumnAutoSelection  {
    return new AdvancedFilterColumnAutoSelection(advancedFilterFormControl.valueChanges, columnsFormControl, mappings, destroy$);
  }
}

export class AdvancedFilterColumnAutoSelection {
  public get listen$(): Observable<IAutoSelectedCriterionForm> {
    return this.filters.pipe(
      debounceTime(100),
      map(form => form.criterionForms),
      map(allCriterions => ({ selection: this.getAutoSelectedCriterion(allCriterions), allCriterions })),
      this.storeAllCriterions,
    );
  }

  private storedCriterions = [];

  constructor(
    private filters: Observable<IAdvancedFilterForm>,
    private facetAndColumnsFC: FormControl,
    private mappings: IAutoSelectedColumnMapping[],
    private destroy$: Subject<void>,
  ) {
    this.listen$
      .pipe(takeUntil(this.destroy$))
      .subscribe(selection => this.updateColumns(selection));
  }

  private updateColumns(selection: IAutoSelectedCriterionForm): void {
    const referencedColumn = this.mappings.find(e => e.criterionKey === selection?.criterion?.key)?.getColumn(selection);

    switch (selection?.state) {
      case IAutoSelectedColumnState.WasAdded:
        this.addColumn(referencedColumn);
        return;
      case IAutoSelectedColumnState.WasRemoved:
        this.removeColumn(referencedColumn);
        return;
      case IAutoSelectedColumnState.Idle:
      default:
        return;
    }
  }

  private addColumn(column: IFacetAndColumn | null): void {
    const currentFacetAndColumns = this.facetAndColumnsFC?.value ?? [];
    const isAlreadySelected = currentFacetAndColumns?.find(c => c?.id === column?.id);
    if (!column || isAlreadySelected) {
      return;
    }

    this.facetAndColumnsFC.patchValue([...currentFacetAndColumns, column]);
  }

  private removeColumn(column: IFacetAndColumn | null): void {
    const currentFacetAndColumns = this.facetAndColumnsFC?.value ?? [];
    const isAlreadyRemoved = currentFacetAndColumns?.every(c => c?.id !== column?.id);
    if (!column || isAlreadyRemoved) {
      return;
    }

    const values = currentFacetAndColumns.filter(v => v.id !== column.id);
    this.facetAndColumnsFC.patchValue(values);
  }

  private getAutoSelectedCriterion(allCriterions: IAdvancedCriterionForm[]): IAutoSelectedCriterionForm {
    const addedCriterion = allCriterions.filter(x => !this.storedCriterions.includes(x))[0];
    const isFirst = !this.storedCriterions?.length && allCriterions?.length === 1;

    if (isFirst || !!addedCriterion) {
      return { ...addedCriterion, state: IAutoSelectedColumnState.WasAdded };
    }

    const removedCriterions = this.storedCriterions.filter(x => !allCriterions.includes(x))[0];
    const isLast = !!this.storedCriterions?.length && !allCriterions?.length;
    if (isLast || !!removedCriterions) {
      return { ...removedCriterions, state: IAutoSelectedColumnState.WasRemoved };
    }

    return { criterion: null, content: null, state: IAutoSelectedColumnState.Idle };
  }

  private get storeAllCriterions():
    UnaryFunction<Observable<IColumnAutoSelectionData>, Observable<IAutoSelectedCriterionForm>> {
    return pipe(tap(d => this.storedCriterions = d.allCriterions), map(d => d.selection));
  }
}
