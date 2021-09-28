import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

import { ContactCategory } from './common/enums/cafe-contacts-category.enum';

@Component({
  selector: 'cc-cafe-contacts',
  templateUrl: './cafe-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CafeContactsComponent {
  @Input() public category: ContactCategory;

  public contactCategory = ContactCategory;
}
