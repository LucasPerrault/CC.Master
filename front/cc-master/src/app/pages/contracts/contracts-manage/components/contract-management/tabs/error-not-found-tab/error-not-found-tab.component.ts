import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ContractManagementService } from '../../contract-management.service';

@Component({
  selector: 'cc-error-not-found-tab',
  templateUrl: './error-not-found-tab.component.html',
  styleUrls: ['error-not-found-tab.component.scss'],
})
export class ErrorNotFoundTabComponent {

  constructor(private activatedRoute: ActivatedRoute, private manageModalService: ContractManagementService) {}

  public close(): void {
    this.manageModalService.close();
  }
}
