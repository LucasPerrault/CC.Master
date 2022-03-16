import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { SelectDisplayMode } from '@cc/common/forms';
import { ILuSidepanelContent, LuSidepanel } from '@lucca-front/ng/sidepanel';
import { Observable, Subject } from 'rxjs';
import { filter, map, take, takeUntil } from 'rxjs/operators';

import { DemoFormKey } from '../../../models/demo-creation-dto.interface';
import { DemoDuplicationsService } from '../../../services/demo-duplications.service';
import { DemosDataService } from '../../../services/demos-data.service';

@Component({
  selector: 'cc-demo-creation-entry-modal',
  template: '',
})
export class DemoCreationEntryModalComponent {

  constructor(
    private luSidepanel: LuSidepanel,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private duplicationsService: DemoDuplicationsService,
  ) {
    const dialog = this.luSidepanel.open(DemoCreationModalComponent);

    dialog.onDismiss.subscribe(async () => await this.redirectToParentAsync());
    dialog.onClose.subscribe(async (duplicationId: string) => {
      this.duplicationsService.add(duplicationId);
      await this.redirectToParentAsync();
    });
  }

  private async redirectToParentAsync(): Promise<void> {
    await this.router.navigate(['.'], {
      relativeTo: this.activatedRoute.parent,
    });
  }
}

@Component({
  selector: 'cc-demo-creation-modal',
  templateUrl: './demo-creation-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DemoCreationModalComponent implements ILuSidepanelContent, OnInit, OnDestroy {
  public title = this.translatePipe.transform('demos_creation_title');
  public submitLabel = this.translatePipe.transform('demos_creation_submit_label');
  public submitDisabled = true;

  public formGroup: FormGroup;
  public formKey = DemoFormKey;

  public showDistributor = new FormControl(false);
  public selectMode = SelectDisplayMode;

  private destroy$ = new Subject<void>();

  constructor(private translatePipe: TranslatePipe, private dataService: DemosDataService) {
    this.formGroup = new FormGroup({
      [DemoFormKey.Subdomain]: new FormControl(),
      [DemoFormKey.Source]: new FormControl(),
      [DemoFormKey.Password]: new FormControl('test'),
      [DemoFormKey.Distributor]: new FormControl(),
      [DemoFormKey.Comment]: new FormControl(),
    });
  }

  public ngOnInit(): void {
    this.formGroup.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.formGroup.invalid);

    this.showDistributor.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(showDistributor => this.setDistributorValidators(showDistributor));

    this.showDistributor.valueChanges
      .pipe(takeUntil(this.destroy$), filter(showDistributor => !showDistributor))
      .subscribe(() => this.resetDistributor());

    this.dataService.getDefaultTemplateDemo$()
      .pipe(take(1))
      .subscribe(s => this.formGroup.get(DemoFormKey.Source).setValue(s));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<string> {
    return this.dataService.create$(this.formGroup.value)
      .pipe(map(duplication => duplication?.instanceDuplicationId));
  }

  public hasRequiredError(formKey: DemoFormKey): boolean {
    const control = this.formGroup.get(formKey);
    return control.touched && control.hasError('required');
  }

  private setDistributorValidators(showDistributor: boolean): void {
    const validators = showDistributor ? [Validators.required] : [];
    const control = this.formGroup.get(DemoFormKey.Distributor);
    control.setValidators(validators);
    control.updateValueAndValidity();
  }

  private resetDistributor(): void {
    this.formGroup.get(DemoFormKey.Distributor).setValue(null);
  }
}
