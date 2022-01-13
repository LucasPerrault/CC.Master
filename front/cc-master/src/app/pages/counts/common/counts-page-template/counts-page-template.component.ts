import { Component } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';
import { INavigationTab } from '@cc/common/navigation';

import { navigationTabs } from '../enums/counts-navigation-path.enum';

@Component({
  selector: 'cc-counts-page-template',
  templateUrl: './counts-page-template.component.html',
  styleUrls: ['./counts-page-template.component.scss'],
})
export class CountsPageTemplateComponent {

  public get tabs(): INavigationTab[] {
    return navigationTabs;
  }

  public get canCreateCounts(): boolean {
    return this.rightsService.hasOperation(Operation.CreateCounts);
  }

  constructor(private rightsService: RightsService) {}
}
