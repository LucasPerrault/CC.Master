import { Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable } from 'rxjs';

import { EstablishmentsDataService } from '../../../services/establishments-data.service';
import { IAttachmentExclusionModalData } from './attachment-exclusion-modal-data.interface';

@Component({
  selector: 'cc-attachment-exclusion-modal',
  templateUrl: './attachment-exclusion-modal.component.html',
})
export class AttachmentExclusionModalComponent implements ILuModalContent {
  public title: string;
  public submitLabel = this.translatePipe.transform('front_contractPage_establishments_exclude_modal_button');

  constructor(
    @Inject(LU_MODAL_DATA) public modalData: IAttachmentExclusionModalData,
    private translatePipe: TranslatePipe,
    private establishmentsDataService: EstablishmentsDataService,
  ) {
    this.title = this.getTitle();
  }

  public submitAction(): Observable<void> {
    const establishmentIds = this.modalData.establishments.map(e => e.id);
    return this.establishmentsDataService.excludeEstablishmentRange$(establishmentIds, this.modalData.product.id);
  }

  public getDescription(): string {
    return this.translatePipe.transform('front_contractPage_establishments_exclude_modal_description', {
      productName: this.modalData.product.name,
      count: this.modalData.establishments.length,
    });
  }

  private getTitle(): string {
    return this.translatePipe.transform('front_contractPage_establishments_exclude_modal_title', {
      count: this.modalData.establishments.length,
    });
  }

}
