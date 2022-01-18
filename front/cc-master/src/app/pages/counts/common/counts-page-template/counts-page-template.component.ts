import { Component } from '@angular/core';
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

}
