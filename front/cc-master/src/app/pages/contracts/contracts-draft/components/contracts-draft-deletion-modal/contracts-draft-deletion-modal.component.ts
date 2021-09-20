import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ToastsService, ToastType } from '@cc/common/toasts';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

import { IContractDraftListEntry } from '../../models';
import { ContractsDraftService } from '../../services';
import { ContractsDraftListService } from '../../services/contracts-draft-list.service';

@Component({
  selector: 'cc-contracts-draft-deletion-modal',
  templateUrl: './contracts-draft-deletion-modal.component.html',
})
export class ContractsDraftDeletionModalComponent implements ILuModalContent {
  public title = this.translatePipe.transform('front_draftPage_deletionModal_title', {
    draftId: this.draft.id,
  });

  public submitLabel = this.translatePipe.transform('front_draftPage_deletionModal_action_label');
  public confirmationMessage = this.translatePipe.transform('front_draftPage_deletionModal_confirmationMessage', {
    clientName: this.draft.client.name,
  });

  constructor(
    private translatePipe: TranslatePipe,
    private toastsService: ToastsService,
    private draftsListService: ContractsDraftListService,
    private draftsService: ContractsDraftService,
    @Inject(LU_MODAL_DATA) public draft: IContractDraftListEntry,
  ) { }

  submitAction(): Observable<void> {
    return this.draftsService.deleteContractDraft$(this.draft.id)
      .pipe(
        catchError((err: HttpErrorResponse) =>
          throwError(`${ err.status }: ${ err.statusText }`),
        ),
        tap(() => {
          this.draftsListService.update();
          this.notifySuccessDeletion();
        }),
      );
  }

  public notifySuccessDeletion(): void {
    this.toastsService.addToast({
      type: ToastType.Success,
      message: this.translatePipe.transform('front_draftPage_deletionModal_successMessage', {
        draftId: this.draft.id,
      }),
    });
  }
}
