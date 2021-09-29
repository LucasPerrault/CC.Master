import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

import { IAdvancedFilterForm } from '../common/cafe-filters/advanced-filter-form';
import { ContactCategory } from './common/enums/cafe-contacts-category.enum';

@Component({
  selector: 'cc-cafe-contacts',
  templateUrl: './cafe-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CafeContactsComponent {
  @Input() public advancedFilterForm: IAdvancedFilterForm;
  @Input() public category: ContactCategory;

  public contactCategory = ContactCategory;
}
