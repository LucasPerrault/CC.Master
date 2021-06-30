import { Injectable } from '@angular/core';
import { Observable, pipe, ReplaySubject, UnaryFunction } from 'rxjs';
import { finalize, map, startWith } from 'rxjs/operators';

import { LifecycleStep } from '../constants/lifecycle-step.enum';
import { ICodeSource } from '../models/code-source.interface';
import { ICodeSourcesFiltered } from '../models/code-sources-filtered.interface';
import { LifecycleStepFilter } from '../models/lifecycle-step-filter.interface';
import { CodeSourcesService } from './code-sources.service';

@Injectable()
export class CodeSourcesListService {
  public get codeSources$(): Observable<ICodeSource[]> {
    return this.codeSourcesSubject$.asObservable();
  }

  public get codeSourcesFiltered$(): Observable<ICodeSourcesFiltered> {
    return this.codeSourcesFilteredSubject$.asObservable();
  }

  public get isLoading$(): Observable<boolean> {
    return this.isLoadingSubject$.asObservable();
  }

  private codeSourcesSubject$: ReplaySubject<ICodeSource[]> = new ReplaySubject<ICodeSource[]>(1);
  private codeSourcesFilteredSubject$: ReplaySubject<ICodeSourcesFiltered> = new ReplaySubject<ICodeSourcesFiltered>(1);
  private isLoadingSubject$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  constructor(private codeSourcesService: CodeSourcesService) {}

  public refresh(): void {
    this.isLoadingSubject$.next(true);

    this.codeSourcesService.getCodeSources$()
      .pipe(
        finalize(() => this.isLoadingSubject$.next(false)),
        startWith([]),
        this.toCodeSourcesFiltered,
      )
      .subscribe(cs => this.codeSourcesFilteredSubject$.next(cs));
  }

  public updateFilters(lifecycle: LifecycleStepFilter): void {
    this.getCodeSourcesByType$(lifecycle).subscribe(cs => this.codeSourcesSubject$.next(cs));
  }

  private getCodeSourcesByType$(lifecycle: LifecycleStepFilter): Observable<ICodeSource[]> {
    switch (lifecycle) {
      case LifecycleStep.Preview:
        return this.codeSourcesFilteredSubject$.pipe(map(f => f.activeCodeSources));
      case LifecycleStep.Referenced:
        return this.codeSourcesFilteredSubject$.pipe(map(f => f.referencedCodeSources));
      case LifecycleStep.Deleted:
        return this.codeSourcesFilteredSubject$.pipe(map(f => f.deletedCodeSources));
      default:
        break;
    }
  }

  private getActiveCodeSources(sources: ICodeSource[]): ICodeSource[] {
    return sources.filter(cs => cs.lifecycle === LifecycleStep.Preview
      || cs.lifecycle === LifecycleStep.ReadyForDeploy
      || cs.lifecycle === LifecycleStep.InProduction,
    );
  }

  private getReferencedCodeSources(sources: ICodeSource[]): ICodeSource[] {
    return sources.filter(cs => cs.lifecycle === LifecycleStep.Referenced);
  }

  private getDeletedCodeSources(sources: ICodeSource[]): ICodeSource[] {
    return sources.filter(cs => cs.lifecycle === LifecycleStep.ToDelete || cs.lifecycle === LifecycleStep.Deleted);
  }

  private get toCodeSourcesFiltered(): UnaryFunction<Observable<ICodeSource[]>, Observable<ICodeSourcesFiltered>> {
    return pipe(map(sources => ({
      activeCodeSources: this.getActiveCodeSources(sources),
      deletedCodeSources: this.getDeletedCodeSources(sources),
      referencedCodeSources: this.getReferencedCodeSources(sources),
    })));
  }
}
