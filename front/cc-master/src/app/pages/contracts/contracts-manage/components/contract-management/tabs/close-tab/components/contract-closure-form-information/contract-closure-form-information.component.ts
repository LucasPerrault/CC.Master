import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import { IClosureFormValidationContext, IContextAttachment } from '../../models/closure-form-validation-context.interface';

@Component({
  selector: 'cc-contract-closure-form-information',
  templateUrl: './contract-closure-form-information.component.html',
})
export class ContractClosureFormInformationComponent {
  @Input() formValidationContext: IClosureFormValidationContext;

  constructor(private translatePipe: TranslatePipe, private datePipe: DatePipe) { }

  public getCloseDateInformation(mostRecentAttachment: IContextAttachment): string {
    const hasEndDate = !!mostRecentAttachment?.end;
    const mostRecentDate = hasEndDate ? mostRecentAttachment?.end : mostRecentAttachment?.start;
    const translationKey = hasEndDate ? 'contracts_closeDate_blockingAttachment_until' : 'contracts_closeDate_blockingAttachment_since';

    return this.translatePipe.transform(translationKey, {
      name: mostRecentAttachment.legalEntity.name,
      id: mostRecentAttachment.legalEntity.id,
      date: this.datePipe.transform(mostRecentDate, 'MMMM yyyy'),
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
