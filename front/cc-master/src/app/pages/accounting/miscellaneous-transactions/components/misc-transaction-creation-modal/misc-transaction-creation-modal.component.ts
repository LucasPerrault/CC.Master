import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent } from '@lucca-front/ng/modal';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, switchMap, takeUntil } from 'rxjs/operators';

import { MiscTransactionInvoiceType } from '../../enums/misc-transaction-invoice-type.enum';
import { IMiscellaneousTransactionForm } from '../../models/miscellaneous-transaction-form.interface';
import { IMiscellaneousTransactionFormContract } from '../../models/miscellaneous-transaction-form-contract.interface';
import { MiscellaneousTransactionsService } from '../../services/miscellaneous-transactions.service';

enum MiscTransactionCreationFormGroupKey {
  Contract = 'contract',
  DocumentLabel = 'documentLabel',
  InvoiceType = 'invoiceType',
  Amount = 'amount',
  PeriodOn = 'periodOn',
  Comment = 'comment'
}

@Component({
  selector: 'cc-misc-transaction-creation-modal',
  templateUrl: './misc-transaction-creation-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MiscTransactionCreationModalComponent implements ILuModalContent, OnInit, OnDestroy {
  public title = this.translatePipe.transform('front_miscTransactions_creation_modal_title');
  public submitLabel = this.translatePipe.transform('front_miscTransactions_creation_modal_submitLabel');
  public submitDisabled = true;

  public formGroup: FormGroup;
  public formGroupKey = MiscTransactionCreationFormGroupKey;
  public invoiceType = MiscTransactionInvoiceType;
  public granularity = ELuDateGranularity;

  public readonly maxCharactersDocumentLabel = 400;

  public miscTransactionContract$: BehaviorSubject<IMiscellaneousTransactionFormContract> = new BehaviorSubject(null);

  public get currencySymbol(): string {
    return this.miscTransactionContract$.value?.offer.currency.symbol;
  }

  public get minPeriodOn(): Date {
    const start = this.miscTransactionContract$.value?.startOn;
    return start ? new Date(start) : null;
  }

  public get maxPeriodOn(): Date {
    const end = this.miscTransactionContract$.value?.closeOn;
    return end ? new Date(end) : null;
  }
  public get hasPeriodOnError(): boolean {
    const periodOn = this.formGroup.get(MiscTransactionCreationFormGroupKey.PeriodOn);
    return periodOn.hasError('min') || periodOn.hasError('max');
  }

  public get selectedContractId(): number {
    return this.formGroup.get(MiscTransactionCreationFormGroupKey.Contract).value?.id;
  }

  public get charactersRemainingOfDocumentLabel(): number {
    const documentLabel = this.formGroup.get(MiscTransactionCreationFormGroupKey.DocumentLabel).value;
    return this.maxCharactersDocumentLabel - documentLabel?.length;
  }

  private destroy$: Subject<void> = new Subject();

  constructor(
    private translatePipe: TranslatePipe,
    private transactionsService: MiscellaneousTransactionsService,
  ) {
    this.formGroup = new FormGroup({
        [MiscTransactionCreationFormGroupKey.Contract]: new FormControl(),
        [MiscTransactionCreationFormGroupKey.DocumentLabel]: new FormControl(''),
        [MiscTransactionCreationFormGroupKey.InvoiceType]: new FormControl(MiscTransactionInvoiceType.Bill),
        [MiscTransactionCreationFormGroupKey.Amount]: new FormControl(),
        [MiscTransactionCreationFormGroupKey.PeriodOn]: new FormControl(),
        [MiscTransactionCreationFormGroupKey.Comment]: new FormControl(),
    });
  }

  public ngOnInit(): void {
    this.formGroup.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.formGroup.invalid);

    this.formGroup.get(MiscTransactionCreationFormGroupKey.Contract).valueChanges
      .pipe(filter(c => !!c), switchMap(contract => this.transactionsService.getContract$(contract.id)))
      .subscribe(currencySymbol => this.miscTransactionContract$.next(currencySymbol));

    this.formGroup.get(MiscTransactionCreationFormGroupKey.Contract).valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.formGroup.get(MiscTransactionCreationFormGroupKey.PeriodOn).reset());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    const form = this.formGroup.value as IMiscellaneousTransactionForm;
    return this.transactionsService.createMiscellaneousTransaction$(form);
  }

  public redirectToContract(): void {
    window.open(this.contractsUrl(this.selectedContractId));
  }

  private contractsUrl = (id: number) => `/${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }/${ id }`;
}
