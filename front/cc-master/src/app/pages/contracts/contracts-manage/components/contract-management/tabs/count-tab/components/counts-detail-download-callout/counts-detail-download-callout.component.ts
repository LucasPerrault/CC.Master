import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { IDetailedCount } from '../../models/detailed-count.interface';

@Component({
  selector: 'cc-counts-detail-download-callout',
  templateUrl: './counts-detail-download-callout.component.html',
})
export class CountsDetailDownloadCalloutComponent {
  @Input() public firstCountWithDetails: IDetailedCount;
  @Output() public download: EventEmitter<void> = new EventEmitter<void>();

  constructor(private datePipe: DatePipe) { }

  public getLastCountPeriod(count: IDetailedCount): string {
    return this.datePipe.transform(count.countPeriod, 'MMM y');
  }

  public downloadCountDetails(): void {
    this.download.emit();
  }
}
