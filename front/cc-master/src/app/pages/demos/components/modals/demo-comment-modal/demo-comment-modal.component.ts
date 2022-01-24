import { Component, Inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import { DemosDataService } from '../../../services/demos-data.service';
import { DemosListService } from '../../../services/demos-list.service';
import { DemoCommentModalMode, IDemoCommentEditionModalData } from './demo-comment-modal-data.interface';

@Component({
  selector: 'cc-demo-comment-modal',
  templateUrl: './demo-comment-modal.component.html',
})
export class DemoCommentModalComponent implements ILuModalContent {
  public title: string;
  public submitLabel: string;

  public comment: FormControl;
  public mode = DemoCommentModalMode;

  constructor(
    @Inject(LU_MODAL_DATA) public data: IDemoCommentEditionModalData,
    private translatePipe: TranslatePipe,
    private dataService: DemosDataService,
    private listService: DemosListService,
  ) {
    this.title = this.translatePipe.transform('demos_comment_modal_submit_title', { subdomain: data?.demo?.subdomain });
    this.submitLabel = this.translatePipe.transform('demos_comment_modal_submit_label');
    this.comment = new FormControl(data?.demo?.comment ?? '');
  }

  public submitAction(): Observable<void> {
    return this.dataService.editComment$(this.data.demo.id, this.comment.value)
      .pipe(tap(() => this.listService.resetAll()));
  }
}
