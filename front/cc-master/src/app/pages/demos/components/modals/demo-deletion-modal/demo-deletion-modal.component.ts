import { Component, Inject } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import { IDemo } from '../../../models/demo.interface';
import { DemosDataService } from '../../../services/demos-data.service';
import { DemosListService } from '../../../services/demos-list.service';

@Component({
  selector: 'cc-demo-deletion-modal',
  templateUrl: './demo-deletion-modal.component.html',
})
export class DemoDeletionModalComponent implements ILuModalContent {
  public title: string;
  public submitLabel: string;

  constructor(
    @Inject(LU_MODAL_DATA) public demo: IDemo,
    private translatePipe: TranslatePipe,
    private dataService: DemosDataService,
    private listService: DemosListService,
  ) {
    this.title = this.translatePipe.transform('demos_deletion_modal_title', { subdomain: demo?.subdomain });
    this.submitLabel = this.translatePipe.transform('demos_deletion_modal_submit_label');
  }

  public submitAction(): Observable<void> {
    return this.dataService.delete$(this.demo.id)
      .pipe(tap(() => this.listService.resetAll()));
  }

}
