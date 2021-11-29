import { Component, Input } from '@angular/core';

@Component({
  selector: 'cc-establishment-type-section',
  templateUrl: './establishment-type-section.component.html',
  styleUrls: ['./establishment-type-section.component.scss'],
})
export class EstablishmentTypeSectionComponent {
  @Input() public title: string;
  @Input() public totalCount: number;
  @Input() public isExpandedByDefault = false;
  @Input() public class: string;

  public toggleEstablishmentSectionDisplay(): void {
    this.isExpandedByDefault = !this.isExpandedByDefault;
  }

}
