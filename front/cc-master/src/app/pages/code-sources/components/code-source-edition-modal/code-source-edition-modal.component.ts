import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuSidepanelContent, LU_SIDEPANEL_DATA, LuSidepanel } from '@lucca-front/ng/sidepanel';
import { combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, take, takeUntil } from 'rxjs/operators';

import { CodeSourceFormKey } from '../../constants/code-source-form-key.enum';
import { CodeSourcesService } from '../../services/code-sources.service';
import { CodeSourcesListService } from '../../services/code-sources-list.service';

@Component({
  selector: 'cc-code-sources-edition-entry-modal',
  template: '',
})
export class CodeSourceEditionEntryModalComponent {

  constructor(
    private luSidepanel: LuSidepanel,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private codeSourcesListService: CodeSourcesListService,
  ) {
    const codeSourceId = this.getCodeSourceId(this.activatedRoute.snapshot.paramMap);
    const dialog = this.luSidepanel.open(CodeSourceEditionModalComponent, codeSourceId);

    dialog.onClose.subscribe(async () => {
      await this.redirectToParentAsync();
      this.codeSourcesListService.refresh();
    });
    dialog.onDismiss.subscribe(async () => await this.redirectToParentAsync());
  }

  private getCodeSourceId(routingParams: ParamMap): number {
    if (!routingParams.has('id')) {
      return null;
    }

    return parseInt(routingParams.get('id'), 10);
  }

  private async redirectToParentAsync(): Promise<void> {
    await this.router.navigate(['.'], {
      relativeTo: this.activatedRoute.parent,
    });
  }
}

@Component({
  selector: 'cc-code-source-edition-modal',
  templateUrl: './code-source-edition-modal.component.html',
})
export class CodeSourceEditionModalComponent implements OnInit, OnDestroy, ILuSidepanelContent {
  public title: string;
  public submitLabel: string;
  public submitDisabled = true;

  public codeSourceForm: FormControl = new FormControl({ value: null, disabled: true });
  public codeSourceFormKey = CodeSourceFormKey;
  public codeSourceSelected: FormControl = new FormControl({ value: null, disabled: true });
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private translatePipe: TranslatePipe,
    private codeSourcesService: CodeSourcesService,
    @Inject(LU_SIDEPANEL_DATA) private codeSourceId: number,
  ) {
    this.title = this.translatePipe.transform('front_sourcePage_editionModal_title');
    this.submitLabel = this.translatePipe.transform('front_sourcePage_editionModal_submit_label');
  }

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.codeSourcesService.getCodeSource$(this.codeSourceId)
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(cs => {
        this.codeSourceSelected.patchValue(cs);
        this.codeSourceForm.patchValue(cs);
      });

    combineLatest([this.codeSourceSelected.valueChanges, this.codeSourceForm.valueChanges])
      .pipe(takeUntil(this.destroy$))
      .subscribe(([initialCodeSource, codeSourceUpdated]) =>
          this.submitDisabled = !codeSourceUpdated?.lifecycle || codeSourceUpdated.lifecycle === initialCodeSource.lifecycle,
      );
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    return this.codeSourcesService.edit$(this.codeSourceId, this.codeSourceForm.value.lifecycle);
  }

  public get githubRepo(): string {
    return this.codeSourceSelected.value?.githubRepo;
  }
}
