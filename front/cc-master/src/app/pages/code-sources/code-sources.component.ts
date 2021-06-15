import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { finalize, map, shareReplay, startWith, switchMap, takeUntil } from 'rxjs/operators';

import { LifecycleStep } from './constants/lifecycle-step.enum';
import { ICodeSource } from './models/code-source.interface';
import { ICodeSourcesFiltered } from './models/code-sources-filtered.interface';
import { LifecycleStepFilter } from './models/lifecycle-step-filter.interface';
import { CodeSourcesListService } from './services/code-sources-list.service';

@Component({
  selector: 'cc-code-sources',
  templateUrl: './code-sources.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CodeSourcesComponent implements OnInit, OnDestroy {

  public codeSourcesListEntries$: ReplaySubject<ICodeSource[]> = new ReplaySubject<ICodeSource[]>(1);
  public codeSourcesFiltered$: ReplaySubject<ICodeSourcesFiltered> = new ReplaySubject<ICodeSourcesFiltered>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public lifecycleFilter: FormControl = new FormControl();

  private codeSources$: Observable<ICodeSource[]>;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private codeSourcesListService: CodeSourcesListService) { }

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.codeSources$ = this.codeSourcesListService.getCodeSources$().pipe(
      finalize(() => this.isLoading$.next(false)),
      startWith([]),
      shareReplay(1),
    );

    this.codeSources$
      .pipe(this.toCodeSourcesFiltered)
      .subscribe(cs => this.codeSourcesFiltered$.next(cs));

    this.lifecycleFilter.valueChanges
      .pipe(takeUntil(this.destroy$), switchMap(lifecycleFilter => this.getCodeSourcesByType$(lifecycleFilter)))
      .subscribe(cs => this.codeSourcesListEntries$.next(cs));
    this.lifecycleFilter.setValue(LifecycleStep.Preview);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private getCodeSourcesByType$(lifecycle: LifecycleStepFilter): Observable<ICodeSource[]> {
    switch (lifecycle) {
      case LifecycleStep.Preview:
        return this.codeSourcesFiltered$.pipe(map(f => f.activeCodeSources));
      case LifecycleStep.Referenced:
        return this.codeSourcesFiltered$.pipe(map(f => f.referencedCodeSources));
      case LifecycleStep.Deleted:
        return this.codeSourcesFiltered$.pipe(map(f => f.deletedCodeSources));
      default:
        break;
    }
  }

  private get toCodeSourcesFiltered(): UnaryFunction<Observable<ICodeSource[]>, Observable<ICodeSourcesFiltered>> {
    return pipe(map(sources => ({
      activeCodeSources: this.codeSourcesListService.getActiveCodeSources(sources),
      deletedCodeSources: this.codeSourcesListService.getDeletedCodeSources(sources),
      referencedCodeSources: this.codeSourcesListService.getReferencedCodeSources(sources),
    })));
  }
}
