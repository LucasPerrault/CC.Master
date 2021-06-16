import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { ILuSidepanelContent, LuSidepanel } from '@lucca-front/ng/sidepanel';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, map, share, take, takeUntil } from 'rxjs/operators';

import { ICodeSource } from '../../models/code-source.interface';
import { CodeSourcesService } from '../../services/code-sources.service';
import { CodeSourcesFetchingService } from '../../services/code-sources-fetching.service';
import { CodeSourcesListService } from '../../services/code-sources-list.service';

@Component({
  selector: 'cc-code-sources-creation-entry-modal',
  template: '',
})
export class CodeSourceCreationEntryModalComponent {

  constructor(
    private luSidepanel: LuSidepanel,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private codeSourcesListService: CodeSourcesListService,
  ) {
    const dialog = this.luSidepanel.open(CodeSourceCreationModalComponent);

    dialog.onClose.subscribe(async () => {
      this.codeSourcesListService.refresh();
      await this.redirectToParentAsync();
    });
    dialog.onDismiss.subscribe(async () => await this.redirectToParentAsync());
  }

  private async redirectToParentAsync(): Promise<void> {
    await this.router.navigate(['.'], {
      relativeTo: this.activatedRoute.parent,
    });
  }
}

@Component({
  selector: 'cc-code-source-creation-modal',
  templateUrl: './code-source-creation-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CodeSourceCreationModalComponent implements OnInit, OnDestroy, ILuSidepanelContent {
  public title: string;
  public submitLabel: string;
  public submitDisabled = true;

  public codeSourceForm: FormControl = new FormControl({ value: null, disabled: true });
  public codeSourceSelected: FormControl = new FormControl();

  public codeSourcesFetched$: ReplaySubject<ICodeSource[]> = new ReplaySubject<ICodeSource[]>(1);
  public fetchButtonState$: Subject<string> = new Subject<string>();
  public githubRepoUrlControl: FormControl = new FormControl('');

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private translatePipe: TranslatePipe,
    private codeSourcesFetchingService: CodeSourcesFetchingService,
    private codeSourcesService: CodeSourcesService,
  ) {
    this.title = this.translatePipe.transform('front_sourcePage_creationModal_title');
    this.submitLabel = this.translatePipe.transform('front_sourcePage_creationModal_submit_label');
  }

  public ngOnInit(): void {
    this.codeSourceForm.valueChanges
      .pipe(takeUntil(this.destroy$), map(form => !form))
      .subscribe(isEmpty => this.submitDisabled = isEmpty);

    this.codeSourceSelected.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(cs => this.codeSourceForm.patchValue(cs));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    return this.codeSourcesService.create$({
      name: this.codeSourceSelected.value.name,
      githubRepo: this.githubRepoUrlControl.value,
      ...this.codeSourceForm.value,
    });
  }

  public fetchGithubData(githubRepoUrl: string): void {
    const sourcesFetched$ = this.codeSourcesFetchingService.getDataFromGithub$(githubRepoUrl)
      .pipe(take(1), share());

    sourcesFetched$
      .pipe(take(1))
      .subscribe(codeSources => {
        this.githubRepoUrlControl.disable();

        this.codeSourcesFetched$.next(codeSources);
        const firstCodeSource = codeSources[0];
        this.codeSourceSelected.patchValue(firstCodeSource);
      });

    sourcesFetched$
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(state => this.fetchButtonState$.next(state));
  }

}
