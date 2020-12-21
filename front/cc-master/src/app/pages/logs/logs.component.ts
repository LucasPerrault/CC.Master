import {Component} from '@angular/core';

@Component({
	selector: 'cc-logs',
	templateUrl: './logs.component.html'
})
export class LogsComponent {
  public get testCount(): number {
    return 10;
  }
}
