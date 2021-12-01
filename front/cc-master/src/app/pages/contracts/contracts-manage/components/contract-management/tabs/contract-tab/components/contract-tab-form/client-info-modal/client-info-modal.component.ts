import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';

import { IClientInfoModalData } from './client-info-modal-data.interface';

@Component({
  selector: 'cc-client-info-modal',
  templateUrl: './client-info-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientInfoModalComponent implements ILuModalContent {
  public title = this.translatePipe.transform('contracts_client_modal_title', { name: this.data.name });

  constructor(
    @Inject(LU_MODAL_DATA) public data: IClientInfoModalData,
    private translatePipe: TranslatePipe,
  ) { }

  public redirectToSalesforceUrl(salesforceId: string): void {
    const salesforceUrl = 'https://eu4.salesforce.com';
    window.open(`${ salesforceUrl }/${ salesforceId }`);
  }

  public copy(text: string): void {
    void navigator.clipboard.writeText(text);
  }
}
