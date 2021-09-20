import { Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable } from 'rxjs';

import { EstablishmentsDataService } from '../../../services/establishments-data.service';
import { IAttachmentDeletionModalData } from './attachment-deletion-modal-data.interface';


@Component({
  selector: 'cc-attachment-deletion-modal',
  templateUrl: './attachment-deletion-modal.component.html',
})
export class AttachmentDeletionModalComponent implements ILuModalContent {
  public title = this.translatePipe.transform('front_contractPage_establishments_delete_modal_title', {
    count: this.modalData.attachmentIds.length,
  });
  public submitLabel = this.translatePipe.transform('front_contractPage_establishments_delete_modal_button');

  constructor(
    @Inject(LU_MODAL_DATA) public modalData: IAttachmentDeletionModalData,
    private translatePipe: TranslatePipe,
    private establishmentsDataService: EstablishmentsDataService,
  ) { }

  public submitAction(): Observable<void> {
    return this.establishmentsDataService.deleteAttachmentRange$(this.modalData.attachmentIds);
  }

  public getDescription(): string {
    return this.translatePipe.transform('front_contractPage_establishments_delete_modal_description', {
      count: this.modalData.attachmentIds.length,
    });
  }
}
