import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Operation, RightsService } from '@cc/aspects/rights';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { LifecycleStep } from './constants/lifecycle-step.enum';
import { ICodeSource } from './models/code-source.interface';
import { ICodeSourcesFiltered } from './models/code-sources-filtered.interface';
import { CodeSourcesListService } from './services/code-sources-list.service';

@Component({
  selector: 'cc-code-sources',
  templateUrl: './code-sources.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CodeSourcesComponent implements OnInit, OnDestroy {
  public lifecycleFilter: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private codeSourcesListService: CodeSourcesListService,
    private rightsService: RightsService,
  ) { }

  public ngOnInit(): void {
    this.lifecycleFilter.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(f => this.codeSourcesListService.updateFilters(f));

    this.lifecycleFilter.setValue(LifecycleStep.Preview);

    this.codeSourcesListService.refresh();
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public get codeSources$(): Observable<ICodeSource[]> {
    return this.codeSourcesListService.codeSources$;
  }

  public get codeSourcesFiltered$(): Observable<ICodeSourcesFiltered> {
    return this.codeSourcesListService.codeSourcesFiltered$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.codeSourcesListService.isLoading$;
  }

  public get canEditCodeSources(): boolean {
    return this.rightsService.hasOperation(Operation.EditCodeSources);
  }
}
