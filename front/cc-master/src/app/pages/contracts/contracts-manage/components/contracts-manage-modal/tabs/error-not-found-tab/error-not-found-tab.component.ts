import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ContractsManageModalService } from '../../contracts-manage-modal.service';

@Component({
  selector: 'cc-error-not-found-tab',
  templateUrl: './error-not-found-tab.component.html',
  styleUrls: ['error-not-found-tab.component.scss'],
})
export class ErrorNotFoundTabComponent {

  constructor(private activatedRoute: ActivatedRoute, private manageModalService: ContractsManageModalService) {}

  public close(): void {
    this.manageModalService.close();
  }
}
