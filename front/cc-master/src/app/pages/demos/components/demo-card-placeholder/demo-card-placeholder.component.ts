import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { IInstanceDuplication } from '@cc/domain/instances';

@Component({
  selector: 'cc-demo-card-placeholder',
  templateUrl: './demo-card-placeholder.component.html',
  styleUrls: ['./demo-card-placeholder.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DemoCardPlaceholderComponent {
  @Input() public title: string;
  @Input() public duplication: IInstanceDuplication;

  public getTitle(subdomain: string): string {
    const domain = 'ilucca-demo.net';
    return `${ subdomain }.${ domain }`;
  }
}
