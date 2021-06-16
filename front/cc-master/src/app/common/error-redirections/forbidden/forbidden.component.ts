import { Component } from '@angular/core';
import { NoNavComponent } from '@cc/common/routing';

@Component({
  selector: 'cc-forbidden',
  templateUrl: './forbidden.component.html',
})
export class ForbiddenComponent extends NoNavComponent {
  public goBack(): void {
    window.history.go(-2);
  }
}
