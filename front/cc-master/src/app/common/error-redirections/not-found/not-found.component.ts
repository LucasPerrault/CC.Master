import { Component } from '@angular/core';
import { NoNavComponent } from '@cc/common/routing';

@Component({
    selector: 'cc-not-found',
    templateUrl: './not-found.component.html',
})
export class NotFoundComponent extends NoNavComponent {
  public goBack(): void {
    window.history.back();
  }
}
