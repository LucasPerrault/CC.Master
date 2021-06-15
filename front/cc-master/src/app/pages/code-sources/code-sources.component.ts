import { Component, OnInit } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { finalize, startWith, take } from 'rxjs/operators';

import { ICodeSource } from './models/code-source.interface';
import { CodeSourcesListService } from './services/code-sources-list.service';

@Component({
  selector: 'cc-code-sources',
  templateUrl: './code-sources.component.html',
})
export class CodeSourcesComponent implements OnInit {

  public codeSources$: ReplaySubject<ICodeSource[]> = new ReplaySubject<ICodeSource[]>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  constructor(private codeSourcesListService: CodeSourcesListService) { }

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.codeSourcesListService.getCodeSources$()
      .pipe(take(1), finalize(() => this.isLoading$.next(false)), startWith([]))
      .subscribe(sources => this.codeSources$.next(sources));
  }

}
