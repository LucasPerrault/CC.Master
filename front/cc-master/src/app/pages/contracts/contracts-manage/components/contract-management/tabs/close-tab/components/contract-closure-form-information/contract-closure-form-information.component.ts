import { DatePipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import { IAttachmentEnded } from '../../models/attachment-ended.interface';
import { IClosureFormValidationContext } from '../../models/closure-form-validation-context.interface';

@Component({
  selector: 'cc-contract-closure-form-information',
  templateUrl: './contract-closure-form-information.component.html',
})
export class ContractClosureFormInformationComponent implements OnInit {
  @Input() lastAttachmentEnded: IAttachmentEnded | null;
  @Input() formValidationContext: IClosureFormValidationContext;

  constructor(private translatePipe: TranslatePipe, private datePipe: DatePipe) { }

  ngOnInit(): void {
  }

  public getCloseDateInformation(): string {
    return this.translatePipe.transform('front_contractPage_closeDateCondition_lastAttachment_callout', {
      name: this.lastAttachmentEnded.legalEntity.name,
      id: this.lastAttachmentEnded.legalEntity.id,
      endDate: this.datePipe.transform(this.lastAttachmentEnded.end, 'dd MMMM yyyy'),
    });
  }

  public getLastCountDateInformation(): string {
    return this.translatePipe.transform('front_contractPage_closeDateCondition_lastCountDate_callout', {
      date: this.datePipe.transform(this.formValidationContext.lastCountPeriod, 'MMMM yyyy'),
    });
  }

  public getTheoreticalStartOnInformation(): string {
    return this.translatePipe.transform('front_contractPage_closeDateCondition_theoreticalStartOn_callout', {
      date: this.datePipe.transform(this.formValidationContext.theoreticalStartOn, 'MMMM yyyy'),
    });
  }

}
