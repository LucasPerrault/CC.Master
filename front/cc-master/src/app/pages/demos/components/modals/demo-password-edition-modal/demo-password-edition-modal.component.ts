import { Component, Inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import { IDemo } from '../../../models/demo.interface';
import { DemosDataService } from '../../../services/demos-data.service';
import { DemosListService } from '../../../services/demos-list.service';

@Component({
  selector: 'cc-demo-password-edition-modal',
  templateUrl: './demo-password-edition-modal.component.html',
})
export class DemoPasswordEditionModalComponent implements ILuModalContent {
  public title: string;
  public submitLabel: string;

  public password: FormControl;

  constructor(
    @Inject(LU_MODAL_DATA) public demo: IDemo,
    private dataService: DemosDataService,
    private listService: DemosListService,
    private translatePipe: TranslatePipe,
  ) {
    this.title = this.translatePipe.transform('demos_password_edition_title', { subdomain: demo?.subdomain });
    this.submitLabel = this.translatePipe.transform('demos_password_edition_submit_label');
    this.password = new FormControl(demo?.instance?.allUsersImposedPassword ?? '');
  }

  public submitAction(): Observable<void> {
    return this.dataService.editPassword$(this.demo.instanceID, this.password.value)
      .pipe(tap(() => this.listService.resetAll()));
  }

}
