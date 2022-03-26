import { BehaviorSubject, Observable } from 'rxjs';
import { debounceTime, map, tap } from 'rxjs/operators';

import { ComparisonCriterion, IAdvancedFilterForm } from '../../../../components/advanced-filter-form';

export enum CriterionSelectionState {
  Idle = 'Idle',
  WasAdded = 'WasAdded',
  WasRemoved = 'WasRemoved',
}

export interface ISelectedCriterionAndState {
  criterion: ComparisonCriterion | null;
  state: CriterionSelectionState;
}

export interface IColumnAutoSelectionOrchestratorData extends ISelectedCriterionAndState {
  allCriterions: ComparisonCriterion[];
}

export class ColumnAutoSelectionOrchestrator {
  public get valueChanges$(): Observable<IColumnAutoSelectionOrchestratorData> {
    return this.advancedFilter$.pipe(
      debounceTime(100),
      map(advancedFilter => this.getAllCriterions(advancedFilter)),
      map(allCriterions => ({ ...this.getSelectedCriterionAndState(allCriterions), allCriterions })),
      tap(d => this.storedCriterions = d.allCriterions),
    );
  }

  private addedColumn$ = new BehaviorSubject<ComparisonCriterion>(null);
  private removedColumn$ = new BehaviorSubject<ComparisonCriterion>(null);

  private storedCriterions = [];

  constructor(private advancedFilter$: Observable<IAdvancedFilterForm>) {}

  private getSelectedCriterionAndState(allCriterions: ComparisonCriterion[]): ISelectedCriterionAndState {
    const addedCriterion = allCriterions.filter(x => !this.storedCriterions.includes(x))[0];
    const isFirst = !this.storedCriterions?.length && allCriterions?.length === 1;
    const shouldAddCriterion = !!addedCriterion && this.addedColumn$.value !== addedCriterion;

    const removedCriterions = this.storedCriterions.filter(x => !allCriterions.includes(x))[0];
    const isLast = !!this.storedCriterions?.length && !allCriterions?.length;
    const shouldRemoveCriterion = !!removedCriterions && this.removedColumn$.value !== removedCriterions;

    if (isFirst || shouldAddCriterion) {
      return { criterion: addedCriterion, state: CriterionSelectionState.WasAdded };
    }

    if (isLast || shouldRemoveCriterion) {
      return { criterion: removedCriterions, state: CriterionSelectionState.WasRemoved };
    }

    return { criterion: null, state: CriterionSelectionState.Idle };
  }

  private getAllCriterions(advancedFilter: IAdvancedFilterForm): ComparisonCriterion[] {
    return advancedFilter.criterionForms.map(c => c?.criterion);
  }
}
